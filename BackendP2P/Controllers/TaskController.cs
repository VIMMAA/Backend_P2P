using Api.Models;
using BackendP2P.Models.Request;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApiB.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    public class TaskController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly ITokenRevocationService _tokenRevocationService;

        public TaskController(ApplicationContext context, ITokenRevocationService tokenRevocationService)
        {
            _context = context;
            _tokenRevocationService = tokenRevocationService;
        }
        
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MaterialWorkModel))]
        [HttpPost("{courseId}/materialWork")]
        public async Task<IActionResult> CreateMaterialWork([FromBody] CombinedMaterialWorkAndCriteriaModels combinedDto, Guid courseId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;

            IActionResult? httpResult = IsForbid(false, courseId);
            if (httpResult != null)
            {
                return httpResult;
            }

            try
            {
                var userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                TaskWorkCreateModel taskDto = combinedDto.MaterialTaskWork;
                List<CriteriaAssignmentCreateModel> criteriaDtos = combinedDto.CriteriaAssignments;
                List<AttachedFileDto> filesDto = combinedDto.Files;

                MaterialWorkModel task = new MaterialWorkModel
                {
                    Id = Guid.NewGuid(),
                    AuthorId = userId,
                    Author = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId),
                    CourseId = courseId,
                    Course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId),
                    Name = taskDto.Name,
                    CreateTime = DateTime.UtcNow,
                    Deadline = taskDto.Deadline,
                    Penalty = taskDto.Penalty == null ? 0.3 : (double)taskDto.Penalty,
                    SolutionsToCheckN = taskDto.SolutionsToCheckN == null && taskDto.isP2P ? 0 : (int)taskDto.SolutionsToCheckN,
                    Check = taskDto.isP2P ? Check.P2P : Check.TeacherOnly,
                    Comments = new List<CommentModel>(),
                    Solutions = new List<SolutionModel>(),
                    Description = taskDto.Description
                };

                List<CriteriaAssignment> criterias = new List<CriteriaAssignment>();

                foreach (CriteriaAssignmentCreateModel criteriaDto in criteriaDtos)
                {
                    CriteriaAssignment criteria = new CriteriaAssignment
                    {
                        Id = Guid.NewGuid(),
                        CountScore = criteriaDto.CountScore,
                        Conditions = criteriaDto.Conditions,
                        GradeModel = null,//PLACE HERE FOREIGN KEY
                        GradeModelId = null,//Nav property here too
                        Title = criteriaDto.Title,
                        MaterialWorkModel = task,
                        MaterialWorkModelId = task.Id
                    };
                    criterias.Add(criteria);
                }

                task.CriteriaAssignments = criterias;

                task.Score = task.CriteriaAssignments.Select(s => s.CountScore).Sum();

                if (task.Deadline < DateTime.UtcNow)
                {
                    return BadRequest("Deadline is outdated");
                }

                List<AttachedFileModel> attachedFiles = new List<AttachedFileModel>();

                foreach (var file in filesDto)
                {
                    AttachedFileModel item = new AttachedFileModel
                    {
                        Id = Guid.NewGuid(),
                        Data = file.Data,
                        Name = file.Name,
                        MaterialWorkId = task.Id,
                        WorkModel = task
                    };
                    attachedFiles.Add(item);
                }

                task.AttachedFiles ??= attachedFiles;

                if (filesDto != null)
                {
                    await _context.AttachedFiles.AddRangeAsync(attachedFiles);
                }

                await _context.MaterialWorks.AddAsync(task);
                await _context.CriteriaAssignments.AddRangeAsync(criterias);
                await _context.SaveChangesAsync();

                return Ok(task);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MaterialReadModel))]
        [HttpPost("{courseId}/materialRead")]
        public async Task<IActionResult> CreateMaterialRead([FromBody] CombinedMaterialReadAndAttachedFiles combinedDto, Guid courseId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;

            IActionResult? httpResult = IsForbid(false, courseId);
            if (httpResult != null)
            {
                return httpResult;
            }

            try
            {
                var userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                MaterialReadCreate materialReadDto = combinedDto.MaterialReadCreate;
                List<AttachedFileDto> filesDto = combinedDto.Files;

                MaterialReadModel materialRead = new MaterialReadModel
                {
                    Id = Guid.NewGuid(),
                    AuthorId = userId,
                    Author = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId),
                    CourseId = courseId,
                    Course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId),
                    Name = materialReadDto.Name,
                    CreateTime = DateTime.UtcNow,
                    Comments = new List<CommentModel>(),
                    Description = materialReadDto.Description
                };

                List<AttachedFileModel> attachedFiles = new List<AttachedFileModel>();

                foreach (var file in filesDto)
                {
                    AttachedFileModel item = new AttachedFileModel
                    {
                        Id = Guid.NewGuid(),
                        Data = file.Data,
                        Name = file.Name,
                        MaterialReadId = materialRead.Id,
                        ReadModel = materialRead
                    };
                    attachedFiles.Add(item);
                }

                materialRead.AttachedFiles ??= attachedFiles;

                if (filesDto != null)
                {
                    await _context.AttachedFiles.AddRangeAsync(attachedFiles);
                }

                await _context.MaterialReads.AddAsync(materialRead);
                await _context.SaveChangesAsync();

                return Ok(materialRead);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MaterialWorkModel))]
        [HttpGet("{taskId}/materialWork")]//check for forbid error
        public async Task<IActionResult> GetMaterialWork(Guid taskId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;

            try
            {
                MaterialWorkModel? task = await _context.MaterialWorks.Include(t => t.Comments).Include(t => t.Solutions).Include(t => t.AttachedFiles).Include(t => t.CriteriaAssignments).ThenInclude(t => t.GradeModel).FirstOrDefaultAsync(t => t.Id == taskId);

                if (task == null)
                    return NotFound(new { message = "Task not found" });

                IActionResult? httpResult = IsForbid(true, task.CourseId);
                if (httpResult != null)
                {
                    return httpResult;
                }

                return Ok(task);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MaterialReadModel))]
        [HttpGet("{taskId}/materialRead")]//check for forbid error
        public async Task<IActionResult> GetMaterialRead(Guid taskId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;

            try
            { 
                MaterialReadModel? task = await _context.MaterialReads.Include(t => t.Comments).Include(t => t.Author).Include(t => t.Course).Include(t => t.AttachedFiles).FirstOrDefaultAsync(t => t.Id == taskId);

                if (task == null)
                    return NotFound(new { message = "Task not found" });

                IActionResult? httpResult = IsForbid(true, task.CourseId);
                if (httpResult != null)
                {
                    return httpResult;
                }

                return Ok(task);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MaterialWorkModel))]
        [HttpPut("{taskId}/materialWork")]
        public async Task<IActionResult> EditMaterialWork(Guid taskId, [FromBody] MaterialWorkEditModel dto)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;

            try
            {
                var userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                MaterialWorkModel? task = await _context.MaterialWorks.Include(t => t.AttachedFiles).Include(t => t.Comments).Include(t => t.Solutions).Include(t => t.CriteriaAssignments).ThenInclude(t => t.GradeModel).Include(t => t.Author).Include(t => t.Course).FirstOrDefaultAsync(t => t.Id == taskId);

                if (task == null)
                    return NotFound(new { message = "Task not found" });


                IActionResult? httpResult = IsForbid(false, task.CourseId);
                if (httpResult != null)
                {
                    return httpResult;
                }
                task.Name = dto.Name;
                task.Deadline = dto.Deadline;
                task.Description = dto.Description;

                if (task.Deadline < DateTime.UtcNow)
                {
                    return BadRequest("Deadline expired");
                }

                _context.MaterialWorks.Update(task);
                await _context.SaveChangesAsync();

                return Ok(task);
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MaterialReadModel))]
        [HttpPut("{taskId}/materialRead")]
        public async Task<IActionResult> EditMaterialRead(Guid taskId, [FromBody] MaterialReadEditModel dto)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;

            try
            {
                var userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                MaterialReadModel? task = await _context.MaterialReads.Include(t => t.AttachedFiles).Include(t => t.Comments).Include(t => t.Author).Include(t => t.Course).FirstOrDefaultAsync(t => t.Id == taskId);

                if (task == null)
                    return NotFound(new { message = "Task not found" });


                IActionResult? httpResult = IsForbid(false, task.CourseId);
                if (httpResult != null)
                {
                    return httpResult;
                }
                task.Name = dto.Name;
                task.Description = dto.Description;

                _context.MaterialReads.Update(task);
                await _context.SaveChangesAsync();

                return Ok(task);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel))]
        [HttpDelete("{taskId}/materialWork")]
        public async Task<IActionResult> DeleteMaterialWork(Guid taskId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;

            try
            {

                MaterialWorkModel? task = await _context.MaterialWorks.Include(t => t.AttachedFiles).Include(t => t.Comments).Include(t => t.Solutions).Include(t => t.CriteriaAssignments).ThenInclude(t => t.GradeModel).FirstOrDefaultAsync(t => t.Id == taskId);

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                IActionResult? httpResult = IsForbid(false, task.CourseId);
                if (httpResult != null)
                {
                    return httpResult;
                }


                _context.MaterialWorks.Remove(task);
                await _context.SaveChangesAsync();

                return Ok(new ResponseModel("Task deleted"));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel))]
        [HttpDelete("{taskId}/materialRead")]
        public async Task<IActionResult> DeleteMaterialRead(Guid taskId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;

            try
            {

                MaterialReadModel? task = await _context.MaterialReads.Include(t => t.AttachedFiles).Include(t => t.Comments).FirstOrDefaultAsync(t => t.Id == taskId);

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                IActionResult? httpResult = IsForbid(false, task.CourseId);
                if (httpResult != null)
                {
                    return httpResult;
                }

                _context.MaterialReads.Remove(task);
                await _context.SaveChangesAsync();

                return Ok(new ResponseModel("Task deleted"));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MaterialWorkModel))]
        [HttpPost("{taskId}/materialWork/Criteria")]
        public async Task<IActionResult> CreateCriteria(Guid taskId, [FromBody] CriteriaAssignmentCreateModel dto)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;

            try
            {
                MaterialWorkModel? task = await _context.MaterialWorks.Include(t => t.AttachedFiles).Include(t => t.Comments).Include(t => t.Solutions).Include(t => t.CriteriaAssignments).ThenInclude(t => t.GradeModel).FirstOrDefaultAsync(t => t.Id == taskId);

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                IActionResult? httpResult = IsForbid(false, task.CourseId);
                if (httpResult != null)
                {
                    return httpResult;
                }


                CriteriaAssignment criteria = new CriteriaAssignment
                {
                    Id = Guid.NewGuid(),
                    Conditions = dto.Conditions,
                    CountScore = dto.CountScore,
                    Title = dto.Title,
                    GradeModel = task.CriteriaAssignments?.FirstOrDefault(c => c.GradeModel != null)?.GradeModel,
                    GradeModelId = task.CriteriaAssignments?.FirstOrDefault(c => c.GradeModelId != null)?.GradeModelId,
                    MaterialWorkModel = task,
                    MaterialWorkModelId = task.Id
                };

                if (task.Deadline < DateTime.UtcNow)
                {
                    return BadRequest("Deadline is outdated");
                }

                task.Score += criteria.CountScore;
                task.CriteriaAssignments?.Add(criteria);

                await _context.CriteriaAssignments.AddAsync(criteria);
                _context.MaterialWorks.Update(task);
                await _context.SaveChangesAsync();

                return Ok(task);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(ResponseModel))]
        [HttpDelete("{taskId}/materialWork/Criteria/{criteriaId}")]
        public async Task<IActionResult> DeleteCriteria(Guid taskId, Guid criteriaId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;
            try
            {
                MaterialWorkModel? task = await _context.MaterialWorks.Include(t => t.AttachedFiles).Include(t => t.Comments).Include(t => t.Solutions).Include(t => t.CriteriaAssignments).ThenInclude(t => t.GradeModel).FirstOrDefaultAsync(t => t.Id == taskId);

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                IActionResult? httpResult = IsForbid(false, task.CourseId);
                if (httpResult != null)
                {
                    return httpResult;
                }

                CriteriaAssignment? criteria = task.CriteriaAssignments.FirstOrDefault(u => criteriaId == u.Id);

                if (criteria == null)
                {
                    return NotFound("Criteria not found");
                }

                task.Score -= criteria.CountScore;
                task.CriteriaAssignments?.Remove(criteria);

                _context.CriteriaAssignments.Remove(criteria);
                _context.MaterialWorks.Update(task);
                await _context.SaveChangesAsync();
                return Ok(new ResponseModel("Criteria deleted"));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MaterialWorkModel))]
        [HttpPut("{taskId}/materialWork/Criteria/{criteriaId}")]
        public async Task<IActionResult> EditCriteria(Guid taskId, Guid criteriaId, [FromBody] CriteriaAssignmentCreateModel dto)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;
            try
            {
                MaterialWorkModel? task = await _context.MaterialWorks.Include(t => t.AttachedFiles).Include(t => t.Comments).Include(t => t.Solutions).Include(t => t.CriteriaAssignments).ThenInclude(t => t.GradeModel).FirstOrDefaultAsync(t => t.Id == taskId);

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                IActionResult? httpResult = IsForbid(false, task.CourseId);
                if (httpResult != null)
                {
                    return httpResult;
                }


                if (task.Deadline < DateTime.UtcNow)
                {
                    return BadRequest("Deadline is outdated");
                }

                CriteriaAssignment? criteria = task.CriteriaAssignments.FirstOrDefault(u => criteriaId == u.Id);

                if (criteria == null)
                {
                    return NotFound("Criteria not found");
                }

                task.Score = task.CriteriaAssignments.Select(s => s.CountScore).Sum();

                criteria.Title = dto.Title;
                criteria.Conditions = dto.Conditions;
                criteria.CountScore = dto.CountScore;

                _context.CriteriaAssignments.Update(criteria);
                _context.MaterialWorks.Update(task);
                await _context.SaveChangesAsync();

                return Ok(task);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CriteriaAssignment))]
        [HttpGet("{taskId}/materialWork/Criteria/{criteriaId}")]
        public async Task<IActionResult> GetCriteria(Guid taskId, Guid criteriaId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;
            try
            {
                MaterialWorkModel? task = await _context.MaterialWorks.Include(t => t.AttachedFiles).Include(t => t.Comments).Include(t => t.Solutions).Include(t => t.CriteriaAssignments).ThenInclude(t => t.GradeModel).FirstOrDefaultAsync(t => t.Id == taskId);

                IActionResult? httpResult = IsForbid(true, task.CourseId);

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                if (httpResult != null)
                {
                    return httpResult;
                }

                CriteriaAssignment? criteria = task.CriteriaAssignments.FirstOrDefault(u => criteriaId == u.Id);

                if (criteria == null)
                {
                    return NotFound("Criteria not found");
                }

                return Ok(criteria);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CriteriaAssignment>))]
        [HttpGet("{taskId}/materialWork/Criteria/List")]
        public async Task<IActionResult> GetCriteriaList(Guid taskId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;
            try
            {
                MaterialWorkModel? task = await _context.MaterialWorks.Include(t => t.AttachedFiles).Include(t => t.Comments).Include(t => t.Solutions).Include(t => t.CriteriaAssignments).ThenInclude(t => t.GradeModel).FirstOrDefaultAsync(t => t.Id == taskId);

                IActionResult? httpResult = IsForbid(true, task.CourseId);

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                if (httpResult != null)
                {
                    return httpResult;
                }

                //if (task.Deadline < DateTime.UtcNow)
                //{
                //    return BadRequest("Deadline is outdated");
                //}

                List<CriteriaAssignment>? criterias = task.CriteriaAssignments.ToList();

                if (criterias == null)
                {
                    return NotFound("Criteria not found");
                }

                return Ok(criterias);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WorkOrRead))]
        [HttpGet("{taskId}/CheckType")]
        public async Task<IActionResult> GetTypeTask(Guid taskId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;
            try
            {
                TaskModel? task = await _context.MaterialWorks.Include(t => t.AttachedFiles).Include(t => t.Comments).Include(t => t.Solutions).Include(t => t.CriteriaAssignments).ThenInclude(t => t.GradeModel).FirstOrDefaultAsync(t => t.Id == taskId);
                WorkOrRead answer = WorkOrRead.Work;

                if (task == null)
                {
                    task = await _context.MaterialReads.Include(t => t.Comments).FirstOrDefaultAsync(t => t.Id == taskId);
                    answer = WorkOrRead.Read;
                }

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                IActionResult? httpResult = IsForbid(true, task.CourseId);

                if (httpResult != null)
                {
                    return httpResult;
                }

                return Ok(answer);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }
        [HttpPost("task/{taskId}/file")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AttachedFileModel))]
        public async Task<IActionResult> AddFileToSolution(Guid taskId, [FromBody] AttachedFileDto file)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;

            try
            {
                var userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                TaskModel? task = await _context.MaterialWorks.Include(t => t.AttachedFiles).Include(t => t.Comments).Include(t => t.Solutions).Include(t => t.CriteriaAssignments).ThenInclude(t => t.GradeModel).FirstOrDefaultAsync(t => t.Id == taskId);
                WorkOrRead type = WorkOrRead.Work;

                if (task == null)
                {
                    task = await _context.MaterialReads.Include(t => t.Comments).FirstOrDefaultAsync(t => t.Id == taskId);
                    type = WorkOrRead.Read;
                }

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                IActionResult? httpResult = IsForbid(true, task.CourseId);

                if (httpResult != null)
                {
                    return httpResult;
                }

                AttachedFileModel answer = new AttachedFileModel
                {
                    Id = Guid.NewGuid(),
                    Data = file.Data,
                    Name = file.Name
                };

                if (type == WorkOrRead.Work)
                {
                    answer.MaterialWorkId = task.Id;
                    answer.WorkModel = (MaterialWorkModel)task;

                    task.AttachedFiles.Add(answer);
                    _context.MaterialWorks.Update((MaterialWorkModel)task);
                } else if (type == WorkOrRead.Read)
                {
                    answer.MaterialReadId = task.Id;
                    answer.ReadModel = (MaterialReadModel)task;

                    task.AttachedFiles.Add(answer);
                    _context.MaterialReads.Update((MaterialReadModel)task);
                }

                _context.AttachedFiles.Add(answer);

                await _context.SaveChangesAsync();

                return Ok(answer);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");
                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }

        [HttpDelete("task/{taskId}/file/{fileId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveFileFromSolution(Guid taskId, Guid fileId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;

            try
            {
                var userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                TaskModel? task = await _context.MaterialWorks.Include(t => t.AttachedFiles).Include(t => t.Comments).Include(t => t.Solutions).Include(t => t.CriteriaAssignments).ThenInclude(t => t.GradeModel).FirstOrDefaultAsync(t => t.Id == taskId);
                WorkOrRead type = WorkOrRead.Work;

                if (task == null)
                {
                    task = await _context.MaterialReads.Include(t => t.Comments).FirstOrDefaultAsync(t => t.Id == taskId);
                    type = WorkOrRead.Read;
                }

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                IActionResult? httpResult = IsForbid(true, task.CourseId);

                if (httpResult != null)
                {
                    return httpResult;
                }

                AttachedFileModel? answer = task.AttachedFiles.FirstOrDefault(f => f.Id == fileId);

                if (answer == null)
                    return NotFound("File not found");

                if (type == WorkOrRead.Work)
                {
                    task.AttachedFiles.Remove(answer);
                    _context.MaterialWorks.Update((MaterialWorkModel)task);
                }
                else if (type == WorkOrRead.Read)
                {
                    task.AttachedFiles.Remove(answer);
                    _context.MaterialReads.Update((MaterialReadModel)task);
                }

                _context.AttachedFiles.Remove(answer);
                await _context.SaveChangesAsync();

                return Ok(new { message = "File removed" });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");
                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }
        private IActionResult? AuthenticateService()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { status = "error", message = "Неавторизованный доступ" });
            }

            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (_tokenRevocationService.IsTokenRevoked(token))
            {
                return Unauthorized(new { status = "error", message = "Неавторизованный доступ" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return BadRequest(new { message = "Invalid token" });
            }

            return null;
        }
        private IActionResult? IsForbid(bool isStudent, Guid courseId)
        {
            var userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            UserCorse? userCorse =  _context.UsersCorses.FirstOrDefault(x => x.UserId == userId && x.CourseId == courseId);

            if (userCorse == null)
            {
                return Forbid();
            }

            
            if (!isStudent)
            {
                Role? role = userCorse.Role;
                if (role == Role.Student || role == null)
                {
                    return Forbid();
                }
            }

            return null;
        }
    }
}
