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

                MaterialWorkModel task = new MaterialWorkModel
                {
                    Id = Guid.NewGuid(),
                    AuthorId = userId,
                    Author = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId),
                    CourseId = courseId,
                    Course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId),
                    Name = taskDto.Name,
                    Topic = taskDto.Topic,
                    CreateTime = DateTime.UtcNow,
                    Deadline = taskDto.Deadline,
                    Check = taskDto.isP2P ? Check.P2P : Check.TeacherOnly,
                    Comments = new List<CommentModel>(),
                    Solutions = new List<SolutionModel>(),
                    Instructions = taskDto.Instructions
                };

                List<CriteriaAssignment> criterias = new List<CriteriaAssignment>();

                foreach (CriteriaAssignmentCreateModel criteriaDto in criteriaDtos)
                {
                    CriteriaAssignment criteria = new CriteriaAssignment
                    {
                        Id = Guid.NewGuid(),
                        CountScore = criteriaDto.CountScore,
                        Conditions = criteriaDto.Conditions,
                        GradeModel = null,
                        GradeModelId = null,
                        Level = criteriaDto.Level,
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
        public async Task<IActionResult> CreateMaterialRead([FromBody] MaterialReadCreate materialReadDto, Guid courseId)
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


                MaterialReadModel materialRead = new MaterialReadModel
                {
                    Id = Guid.NewGuid(),
                    AuthorId = userId,
                    Author = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId),
                    CourseId = courseId,
                    Course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId),
                    Name = materialReadDto.Name,
                    Topic = materialReadDto.Topic,
                    CreateTime = DateTime.UtcNow,
                    Comments = new List<CommentModel>()
                };

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
                MaterialWorkModel? task = await _context.MaterialWorks.Include(t => t.Comments).Include(t => t.Solutions).Include(t => t.CriteriaAssignments).ThenInclude(t => t.GradeModel).FirstOrDefaultAsync(t => t.Id == taskId);

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
                MaterialReadModel? task = await _context.MaterialReads.Include(t => t.Comments).Include(t => t.Author).Include(t => t.Course).FirstOrDefaultAsync(t => t.Id == taskId);

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

                MaterialWorkModel? task = await _context.MaterialWorks.Include(t => t.Comments).Include(t => t.Solutions).Include(t => t.CriteriaAssignments).ThenInclude(t => t.GradeModel).Include(t => t.Author).Include(t => t.Course).FirstOrDefaultAsync(t => t.Id == taskId);

                if (task == null)
                    return NotFound(new { message = "Task not found" });


                IActionResult? httpResult = IsForbid(false, task.CourseId);
                if (httpResult != null)
                {
                    return httpResult;
                }
                task.Name = dto.Name;
                task.Topic = dto.Topic;
                task.Deadline = dto.Deadline;
                task.Instructions = dto.Instructions;

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

                MaterialReadModel? task = await _context.MaterialReads.Include(t => t.Comments).Include(t => t.Author).Include(t => t.Course).FirstOrDefaultAsync(t => t.Id == taskId);

                if (task == null)
                    return NotFound(new { message = "Task not found" });


                IActionResult? httpResult = IsForbid(false, task.CourseId);
                if (httpResult != null)
                {
                    return httpResult;
                }
                task.Name = dto.Name;
                task.Topic = dto.Topic;
                task.Content = dto.Content;

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

                MaterialWorkModel? task = await _context.MaterialWorks.Include(t => t.Comments).Include(t => t.Solutions).Include(t => t.CriteriaAssignments).ThenInclude(t => t.GradeModel).FirstOrDefaultAsync(t => t.Id == taskId);

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

                MaterialReadModel? task = await _context.MaterialReads.Include(t => t.Comments).FirstOrDefaultAsync(t => t.Id == taskId);

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
        [HttpPost("{taskId}/workMaterial/Criteria")]
        public async Task<IActionResult> CreateCriteria(Guid taskId, [FromBody] CriteriaAssignmentCreateModel dto)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;

            try
            {
                MaterialWorkModel? task = await _context.MaterialWorks.Include(t => t.Comments).Include(t => t.Solutions).Include(t => t.CriteriaAssignments).ThenInclude(t => t.GradeModel).FirstOrDefaultAsync(t => t.Id == taskId);

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
                    Level = dto.Level,
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
        [HttpDelete("{taskId}/workMaterial/Criteria/{criteriaId}")]
        public async Task<IActionResult> DeleteCriteria(Guid taskId, Guid criteriaId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;
            try
            {
                MaterialWorkModel? task = await _context.MaterialWorks.Include(t => t.Comments).Include(t => t.Solutions).Include(t => t.CriteriaAssignments).ThenInclude(t => t.GradeModel).FirstOrDefaultAsync(t => t.Id == taskId);

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
        [HttpPut("{taskId}/workMaterial/Criteria/{criteriaId}")]
        public async Task<IActionResult> EditCriteria(Guid taskId, Guid criteriaId, [FromBody] CriteriaAssignmentCreateModel dto)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;
            try
            {
                MaterialWorkModel? task = await _context.MaterialWorks.Include(t => t.Comments).Include(t => t.Solutions).Include(t => t.CriteriaAssignments).ThenInclude(t => t.GradeModel).FirstOrDefaultAsync(t => t.Id == taskId);

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
                criteria.Level = dto.Level;
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
        [HttpGet("{taskId}/workMaterial/Criteria/{criteriaId}")]
        public async Task<IActionResult> GetCriteria(Guid taskId, Guid criteriaId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;
            try
            {
                MaterialWorkModel? task = await _context.MaterialWorks.Include(t => t.Comments).Include(t => t.Solutions).Include(t => t.CriteriaAssignments).ThenInclude(t => t.GradeModel).FirstOrDefaultAsync(t => t.Id == taskId);

                IActionResult? httpResult = IsForbid(false, task.CourseId);

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
        [HttpGet("{taskId}/workMaterial/Criteria/List")]
        public async Task<IActionResult> GetCriteriaList(Guid taskId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;
            try
            {
                MaterialWorkModel? task = await _context.MaterialWorks.Include(t => t.Comments).Include(t => t.Solutions).Include(t => t.CriteriaAssignments).ThenInclude(t => t.GradeModel).FirstOrDefaultAsync(t => t.Id == taskId);

                IActionResult? httpResult = IsForbid(false, task.CourseId);

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                if (httpResult != null)
                {
                    return httpResult;
                }

                if (task.Deadline < DateTime.UtcNow)
                {
                    return BadRequest("Deadline is outdated");
                }

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
