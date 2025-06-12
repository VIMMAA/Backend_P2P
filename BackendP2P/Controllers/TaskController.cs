using System.Security.Claims;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Enums;

namespace ApiB.Controllers
{
    [Route("api/[controller]")]
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

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("{courseId}/create")]
        [Authorize]
        public async Task<IActionResult> CreateTask([FromBody] TaskCreateModel model, Guid courseId)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (_tokenRevocationService.IsTokenRevoked(token))
            {
                return Unauthorized(new { status = "error", message = "Unauthorized access" });
            }

            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return BadRequest(new { status = "error", message = "Invalid token" });
            }

            bool hasAccess = await _context.UsersCorses.AnyAsync(u =>
                (u.Role == Role.Owner || u.Role == Role.Teacher) &&
                u.UserId.ToString() == userIdClaim.Value &&
                u.CourseId == courseId
            );

            if (!hasAccess)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "error", message = "Invalid arguments" });
            }

            if (!await _context.Courses.AnyAsync(c => c.Id == courseId))
            {
                return BadRequest(new { status = "error", message = "Course with this id does not exist" });
            }

            var userId = Guid.Parse(userIdClaim.Value);

            var task = new TaskModel
            {
                Id = Guid.NewGuid(),
                AuthorId = userId,
                CourseId = courseId,
                Name = model.Title,
                StudentGroup = model.StudentGroup,
                Solution = null,
                Comments = null,
                Topic = model.Topic,
                CreateTime = DateTime.UtcNow,
                Deadline = model.DueDate
            };

            try
            {
                await _context.Tasks.AddAsync(task);
                await _context.SaveChangesAsync();

                return Ok(new { status = "success", taskId = task.Id });
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error creating task: {e}");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

                [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetTask(Guid id)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (_tokenRevocationService.IsTokenRevoked(token))
            {
                return Unauthorized(new { status = "error", message = "Unauthorized access" });
            }

            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return BadRequest(new { status = "error", message = "Invalid token" });
            }

            bool exists = await _context.Tasks.AnyAsync(t => t.Id == id);

            if (!exists)
            {
                return BadRequest(new { status = "error", message = "This task does not exists" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "error", message = "Invalid arguments" });
            }

            try
            {
                TaskModel? task = await _context.Tasks.FirstOrDefaultAsync(c => c.Id == id);

                
                TaskGetModel answer = new TaskGetModel
                {
                    Name = task.Name,
                    Topic = task.Topic,
                    StudentGroup = task.StudentGroup,
                    Deadline = task.Deadline
                };

                return Ok(answer);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error creating task: {e}");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }
    }
}
