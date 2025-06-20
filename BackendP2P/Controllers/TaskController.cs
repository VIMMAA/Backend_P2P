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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TaskCreatedModel))]
        [HttpPost("{courseId}")]
        public async Task<IActionResult> CreateTask([FromBody] TaskCreateModel model, Guid courseId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;

            try
            {
                var userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                TaskModel task = new TaskModel
                {
                    Id = Guid.NewGuid(),
                    AuthorId = userId,
                    Author = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId),
                    Students = model.Students?.ToList() ?? new List<Guid>(),
                    CourseId = courseId,
                    Course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId),
                    Name = model.Name,
                    Topic = model.Topic,
                    CreateTime = DateTime.UtcNow,
                    Deadline = model.Deadline,
                    Comments = new List<CommentModel>(),
                    Solutions = new List<SolutionModel>(),
                    Grades = new List<GradeModel>()
                };

                foreach (var studentId in task.Students)
                {
                    if (task.Students.Count != task.Students.Distinct().Count())
                        return BadRequest(new { message = "Duplicate students in the list." });
                    if (!await _context.UsersCorses.AnyAsync(s => s.UserId == studentId && task.CourseId == s.CourseId && s.Role == Role.Student))
                        return BadRequest(new { message = "At least one of the students not found on this course" });

                }

                TaskCreatedModel answer = new TaskCreatedModel
                {
                    Id = task.Id,
                    AuthorId = task.AuthorId,
                    Students = task.Students,
                    CourseId = task.CourseId,
                    Name = task.Name,
                    Topic = task.Topic,
                    CreateTime = task.CreateTime,
                    Deadline = task.Deadline,
                    Comments = new List<CommentModel>(),
                    Solutions = new List<SolutionModel>(),
                    Grades = new List<GradeModel>()
                };

                await _context.Tasks.AddAsync(task);
                await _context.SaveChangesAsync();

                return Ok(answer);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TaskCreatedModel))]
        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetTask(Guid courseId, Guid taskId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;

            try
            {
                TaskModel task = await _context.Tasks.Include(t => t.Comments).Include(t => t.Solutions).Include(g => g.Grades).FirstOrDefaultAsync(t => t.Id == taskId);
                if (task == null)
                    return NotFound(new { message = "Task not found" });

                TaskCreatedModel answer = new TaskCreatedModel
                {
                    Id = task.Id,
                    AuthorId = task.AuthorId,
                    Students = task.Students,
                    CourseId = task.CourseId,
                    Name = task.Name,
                    Topic = task.Topic,
                    CreateTime = task.CreateTime,
                    Deadline = task.Deadline,
                    Comments = task.Comments.Select(c => new CommentModel
                    {
                        Id = c.Id,
                        Text = c.Text,
                        AuthorId = c.AuthorId,
                        CreateTime = c.CreateTime
                    }).ToList(),
                    Solutions = task.Solutions.Select(s => new SolutionModel { 
                        Id = s.Id,
                        StudentId = s.StudentId,
                        Content = s.Content,
                        AttachmentPath = s.AttachmentPath,
                        TaskId = s.TaskId,
                    }).ToList(),
                    Grades = task.Grades.Select(g => new GradeModel
                    {
                        Id = g.Id,
                        TeacherId = g.TeacherId,
                        Score = g.Score,
                        StudentId = g.StudentId,
                    }).ToList(),
                    MaterialReadId = task.MaterialReadId,
                    MaterialWorkId = task.MaterialWorkId,
                };

                return Ok(answer);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel))]
        [HttpPut("{taskId}")]
        public async Task<IActionResult> EditTask(Guid courseId, Guid taskId, [FromBody] TaskEditModel dto)
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

            try
            {
                TaskModel task = await _context.Tasks.Include(t => t.Comments).Include(t => t.Solutions).Include(g => g.Grades).FirstOrDefaultAsync(t => t.Id == taskId);
                if (task == null)
                    return NotFound(new { message = "Task not found" });

                task.Name = dto.Name;
                task.Students = dto.Students;
                task.Topic = dto.Topic;
                task.Deadline = dto.Deadline;

                foreach (var studentId in task.Students)
                {
                    if (task.Students.Count != task.Students.Distinct().Count())
                        return BadRequest(new { message = "Duplicate students in the list." });
                    if (!await _context.UsersCorses.AnyAsync(s => s.UserId == studentId && task.CourseId == s.CourseId && s.Role == Role.Student))
                        return BadRequest(new { message = "One of the students not found in StudentCorse" });

                }

                _context.Tasks.Update(task);
                await _context.SaveChangesAsync();

                return Ok(new ResponseModel("Task updated"));
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel))]
        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask(Guid courseId, Guid taskId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;

            try
            {
                TaskModel? task = await _context.Tasks.FirstOrDefaultAsync(u => u.Id == taskId);

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();

                return Ok(new ResponseModel("Task deleted"));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MaterialReadModel))]
        [HttpPost("{taskId}/readMaterial")]
        public async Task<IActionResult> CreateReadTask(Guid taskId, [FromBody] TaskReadCreateDto taskRead)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;

            try
            {
                TaskModel? task = await _context.Tasks.FirstOrDefaultAsync(u => u.Id == taskId);

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                if (await _context.MaterialReads.AnyAsync(u => u.TaskId == taskId))
                {
                    return BadRequest("Task already have read material");
                }

                MaterialReadModel updateRead = new MaterialReadModel
                {
                    Id = Guid.NewGuid(),
                    TaskId = taskId,
                    Task = task,
                    Content = taskRead.Content
                };
                
                task.MaterialReadId = updateRead.Id;
                task.MaterialReadModel = updateRead;

                _context.Tasks.Update(task);
                await _context.MaterialReads.AddAsync(updateRead);
                await _context.SaveChangesAsync();

                return Ok(updateRead);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel))]
        [HttpDelete("{taskId}/readMaterial")]
        public async Task<IActionResult> DeleteReadTask(Guid taskId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;
            try
            {
                TaskModel? task = await _context.Tasks.FirstOrDefaultAsync(u => u.Id == taskId);

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                MaterialReadModel? readModel = await _context.MaterialReads.FirstOrDefaultAsync(u => u.TaskId == taskId);

                if (readModel == null)
                {
                    return BadRequest("Task doesn't have any read material. Deleting is impossible.");
                }

                task.MaterialReadId = null;
                task.MaterialReadModel = null;

                _context.MaterialReads.Remove(readModel);
                _context.Tasks.Update(task);
                await _context.SaveChangesAsync();

                return Ok(new ResponseModel($"Read material {readModel.Id} deleted"));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel))]
        [HttpPatch("{taskId}/readMaterial/change")]
        public async Task<IActionResult> ReplaceReadTask(Guid taskId, [FromBody] TaskReadCreateDto taskRead)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;

            try
            {
                TaskModel? task = await _context.Tasks.FirstOrDefaultAsync(u => u.Id == taskId);

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                MaterialReadModel? readModel = await _context.MaterialReads.FirstOrDefaultAsync(u => u.TaskId == taskId);

                if (readModel == null)
                {
                    return BadRequest("Task doesn't have any read material. Replacing is impossible.");
                }

                _context.MaterialReads.Remove(readModel);

                MaterialReadModel updateRead = new MaterialReadModel
                {
                    Id = Guid.NewGuid(),
                    TaskId = taskId,
                    Task = task,
                    Content = taskRead.Content
                };

                await _context.MaterialReads.AddAsync(updateRead);

                task.MaterialReadId = updateRead.Id;
                task.MaterialReadModel = updateRead;

                _context.Tasks.Update(task);
                await _context.SaveChangesAsync();

                return Ok(new ResponseModel($"Read material {readModel.Id} was replaced by {updateRead.Id}"));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MaterialReadModel))]
        [HttpGet("{taskId}/readMaterial")]
        public async Task<IActionResult> GetReadTask(Guid taskId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;

            try
            {
                TaskModel? task = await _context.Tasks.FirstOrDefaultAsync(u => u.Id == taskId);

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                MaterialReadModel? readModel = await _context.MaterialReads.FirstOrDefaultAsync(u => u.TaskId == taskId);

                if (readModel == null)
                {
                    return BadRequest("Task doesn't have any read material.");
                }

                return Ok(readModel);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MaterialWorkModel))]
        [HttpPost("{taskId}/workMaterial")]
        public async Task<IActionResult> CreateWorkTask(Guid taskId, [FromBody] TaskWorkCreateModel taskWork)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;
            
            try
            {
                TaskModel? task = await _context.Tasks.FirstOrDefaultAsync(u => u.Id == taskId);

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                if (await _context.MaterialWorks.AnyAsync(u => u.TaskId == taskId))
                {
                    return BadRequest("Task already have work material");
                }

                MaterialWorkModel updateWork = new MaterialWorkModel
                {
                    Id = Guid.NewGuid(),
                    TaskId = taskId,
                    Task = task,
                    Score = taskWork.Score,
                    Deadline = task.Deadline,
                    Instructions = taskWork.Instructions,
                    CriteriaAssignments = null//пока null
                };

                task.MaterialWorkId = updateWork.Id;
                task.MaterialWorkModel = updateWork;

                _context.Tasks.Update(task);
                await _context.MaterialWorks.AddAsync(updateWork);
                await _context.SaveChangesAsync();

                return Ok(updateWork);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel))]
        [HttpDelete("{taskId}/workMaterial")]
        public async Task<IActionResult> DeleteWorkTask(Guid taskId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;

            try
            {
                TaskModel? task = await _context.Tasks.FirstOrDefaultAsync(u => u.Id == taskId);

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                MaterialWorkModel materialWork = await _context.MaterialWorks.FirstOrDefaultAsync(u => u.TaskId == taskId);

                if (materialWork == null)
                {
                    return NotFound("Material work not found");
                }

                _context.MaterialWorks.Remove(materialWork);

                task.MaterialWorkId = null;
                task.MaterialWorkModel = null;

                _context.Tasks.Update(task);
                await _context.SaveChangesAsync();

                return Ok(new ResponseModel("Work task deleted"));
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
    }
}
