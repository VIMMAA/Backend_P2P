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

        [HttpPost("{courseId}/create")]
        [Authorize]
        public async Task<IActionResult> CreateTask([FromBody] TaskCreateModel model, Guid courseId)
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
                    Students = new List<Guid>(),
                    CourseId = model.CourseId,
                    Course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == model.CourseId),
                    Name = model.Name,
                    Topic = model.Topic,
                    CreateTime = model.CreateTime,
                    Deadline = model.Deadline,
                    Comments = new List<CommentModel>(),
                    Solutions = new List<SolutionModel>(),
                    Grades = new List<GradeModel>(),
                    MaterialReadId = model.MaterialReadId,
                    MaterialReadModel = await _context.MaterialReads.FirstOrDefaultAsync(m => m.Id == model.MaterialReadId),
                    MaterialWorkId = model.MaterialWorkId,
                    MaterialWorkModel = await _context.MaterialWorks.FirstOrDefaultAsync(m => m.Id == model.MaterialWorkId)
                };

                TaskCreatedModel answer = new TaskCreatedModel
                {
                    Id = task.Id,
                    AuthorId = userId,
                    Students = new List<Guid>(),
                    CourseId = model.CourseId,
                    Name = model.Name,
                    Topic = model.Topic,
                    CreateTime = model.CreateTime,
                    Deadline = model.Deadline,
                    Comments = new List<CommentModel>(),
                    Solutions = new List<SolutionModel>(),
                    Grades = new List<GradeModel>(),
                    MaterialReadId = model.MaterialReadId,
                    MaterialWorkId = model.MaterialWorkId,
                };

                await _context.Tasks.AddAsync(task);
                await _context.SaveChangesAsync();

                return Ok(answer);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error registering user: {ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }
    }
}
