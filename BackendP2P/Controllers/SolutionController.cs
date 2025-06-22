using Api.Models;
using BackendP2P.Models.Request;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackendP2P.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    public class SolutionController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly ITokenRevocationService _tokenRevocationService;
        public SolutionController(ApplicationContext context, ITokenRevocationService tokenRevocationService)
        {
            _context = context;
            _tokenRevocationService = tokenRevocationService;
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SolutionModel))]
        [HttpPost("{taskId}")]
        public async Task<IActionResult> CreateSolution([FromBody] SolutionCreateModel model, Guid taskId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;

            try
            {
                var userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                TaskModel task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);

                IActionResult? httpResult = IsForbid(true, task.CourseId);
                if (httpResult != null)
                {
                    return httpResult;
                }

                if (!task.Students.Contains(userId))
                {
                    return StatusCode(403, "Student is not on the list to submit task");
                }

                if (task.Deadline < DateTime.UtcNow)
                {
                    return BadRequest("Deadline expired");
                }

                SolutionModel solution = new SolutionModel
                {
                    Id = Guid.NewGuid(),
                    StudentId = userId,
                    Student = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId),
                    SubmissionTime = DateTime.UtcNow,
                    AttachmentPath = model.AttachmentPath,
                    Content = model.Content,
                    TaskId = taskId,
                    Task = task
                };

                task.Solutions ??= new List<SolutionModel>();
                task.Solutions.Add(solution);

                _context.Tasks.Update(task);
                await _context.Solutions.AddAsync(solution);
                await _context.SaveChangesAsync();

                return Ok(solution);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }

        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(ResponseModel))]
        [HttpDelete("{taskId}/{solutionId}")]
        public async Task<IActionResult> DeleteSolution(Guid taskId, Guid solutionId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;

            try
            {
                var userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                TaskModel task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);

                IActionResult? httpResult = IsForbid(true, task.CourseId);
                if (httpResult != null)
                {
                    return httpResult;
                }

                SolutionModel solution = await _context.Solutions.FirstOrDefaultAsync(solution => solution.Id == solutionId);

                if (solution == null)
                {
                    return NotFound("Solution not found");
                }

                if (solution.StudentId != userId)
                {
                    return Forbid();
                }

                if (!task.Students.Contains(userId))
                {
                    return StatusCode(403, "Student is not on the list to submit task");
                }

                if (task.Deadline < DateTime.UtcNow)
                {
                    return BadRequest("Deadline expired. You can not delete solution");
                }

                task.Solutions.Remove(solution);

                _context.Tasks.Update(task);
                _context.Solutions.Remove(solution);
                await _context.SaveChangesAsync();

                return Ok(new ResponseModel("Solution deleted"));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SolutionModel))]
        [HttpPut("{taskId}/{solutionId}")]
        public async Task<IActionResult> PutSolution (Guid taskId, Guid solutionId, [FromBody] SolutionCreateModel dto)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;

            try
            {
                var userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                TaskModel task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);

                IActionResult? httpResult = IsForbid(true, task.CourseId);
                if (httpResult != null)
                {
                    return httpResult;
                }

                SolutionModel solution = await _context.Solutions.FirstOrDefaultAsync(solution => solution.Id == solutionId);

                if (solution == null)
                {
                    return NotFound("Solution not found");
                }

                if (solution.StudentId != userId)
                {
                    return Forbid();
                }

                if (!task.Students.Contains(userId))
                {
                    return StatusCode(403, "Student is not on the list to submit task");
                }

                if (task.Deadline < DateTime.UtcNow)
                {
                    return BadRequest("Deadline expired. You can not update solution");
                }

                solution.Content = dto.Content;
                solution.AttachmentPath = dto.AttachmentPath;

                _context.Solutions.Update(solution);
                await _context.SaveChangesAsync();

                return Ok(solution);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SolutionModel))]
        [HttpGet("{taskId}/{solutionId}")]
        public async Task<IActionResult> GetSolution(Guid taskId, Guid solutionId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;

            try
            {
                var userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                TaskModel task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);

                IActionResult? httpResult = IsForbid(true, task.CourseId);

                if (httpResult != null)
                {
                    return httpResult;
                }

                SolutionModel solution = await _context.Solutions.FirstOrDefaultAsync(solution => solution.Id == solutionId);

                if (solution == null)
                {
                    return NotFound("Solution not found");
                }

                if (!(solution.StudentId == userId || await _context.UsersCorses.AnyAsync(u => u.UserId == userId && (u.Role == Role.Teacher || u.Role == Role.Owner))))
                {
                    return StatusCode(403, "You are not an author student nor an owner nor a teacher");
                }

                return Ok(solution);
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

            UserCorse? userCorse = _context.UsersCorses.FirstOrDefault(x => x.UserId == userId && x.CourseId == courseId);

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
