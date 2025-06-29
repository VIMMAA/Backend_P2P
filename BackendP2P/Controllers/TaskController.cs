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
                    SolutionsToCheckN = taskDto.SolutionsToCheckN == null ? 0 : (int)taskDto.SolutionsToCheckN,
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


                var solutionIds = task.Solutions.Select(s => s.Id).ToList();

                var filesFromSolutions = await _context.AttachedFiles
                    .Where(f => solutionIds.Contains(f.SolutionId!.Value))
                    .ToListAsync();

                _context.AttachedFiles.RemoveRange(filesFromSolutions);
                _context.AttachedFiles.RemoveRange(task.AttachedFiles);

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

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MaterialWorkModel))]
        [HttpPut("{courseId}/materialWork/{taskId}")]
        public async Task<IActionResult> MaterialWorkEdit([FromBody] CombinedMaterialWorkEdit combinedDto, Guid courseId, Guid taskId)
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
                MaterialWorkModel? task = await _context.MaterialWorks.Include(t => t.AttachedFiles).Include(t => t.Comments).Include(t => t.Solutions).Include(t => t.CriteriaAssignments).ThenInclude(t => t.GradeModel).FirstOrDefaultAsync(t => t.Id == taskId);

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                var userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                MaterialWorkEditModel taskDto = combinedDto.MaterialWorkEdit;
                List<CriteriaAssignmentCreateModel> criteriaDtos = combinedDto.CriteriaAssignments;
                List<AttachedFileDto> filesDto = combinedDto.Files;

                List<AttachedFileModel> filesToDelete = task.AttachedFiles.ToList();
                List<CriteriaAssignment> criteriasToDelete = task.CriteriaAssignments.ToList();

                foreach (var file in filesToDelete)
                {
                    task.AttachedFiles.Remove(file);
                }
                 
                foreach (var file in criteriasToDelete)
                {
                    task.CriteriaAssignments.Remove(file);
                }

                _context.AttachedFiles.RemoveRange(filesToDelete);
                _context.CriteriaAssignments.RemoveRange(criteriasToDelete);

                task.Description = taskDto.Description;
                task.Deadline = taskDto.Deadline;
                task.Name = taskDto.Name;

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

                _context.MaterialWorks.Update(task);
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
        [HttpPut("{courseId}/materialRead/{taskId}")]
        public async Task<IActionResult> CreateMaterialRead([FromBody] CombinedMaterialReadAndAttachedFiles combinedDto, Guid courseId, Guid taskId)
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

                MaterialReadModel? task = await _context.MaterialReads.Include(t => t.AttachedFiles).Include(t => t.Comments).FirstOrDefaultAsync(t => t.Id == taskId);

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                task.Description = materialReadDto.Description;
                task.Name = materialReadDto.Name;

                List<AttachedFileModel> filesToDelete = task.AttachedFiles.ToList();
                
                foreach (var file in filesToDelete)
                {
                    task.AttachedFiles.Remove(file);
                }

                _context.AttachedFiles.RemoveRange(filesToDelete);

                List<AttachedFileModel> attachedFiles = new List<AttachedFileModel>();

                foreach (var file in filesDto)
                {
                    AttachedFileModel item = new AttachedFileModel
                    {
                        Id = Guid.NewGuid(),
                        Data = file.Data,
                        Name = file.Name,
                        MaterialReadId = task.Id,
                        ReadModel = task
                    };
                    attachedFiles.Add(item);
                }

                task.AttachedFiles ??= attachedFiles;

                if (filesDto != null)
                {
                    await _context.AttachedFiles.AddRangeAsync(attachedFiles);
                }

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
