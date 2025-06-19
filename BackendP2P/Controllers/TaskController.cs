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
        [HttpPost("{courseId}")]
        public async Task<ActionResult<TaskCreatedModel>> CreateTask([FromBody] TaskCreateModel model, Guid courseId)
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
                var userId = Guid.Parse(userIdClaim.Value);

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
        [HttpGet("{taskId}")]
        public async Task<ActionResult<TaskCreatedModel>> GetTask(Guid courseId, Guid taskId)
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

        [HttpPut("{taskId}")]
        public async Task<ActionResult<ResponseModel>> EditTask(Guid courseId, Guid taskId, [FromBody] TaskEditModel dto)
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

        [HttpDelete("{taskId}")]
        public async Task<ActionResult<ResponseModel>> DeleteTask(Guid courseId, Guid taskId)
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
        [HttpPost("{taskId}/readMaterial")]
        public async Task<ActionResult<MaterialReadModel>> CreateReadTask(Guid taskId, [FromBody] TaskReadCreateDto taskRead)
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
    }
}
