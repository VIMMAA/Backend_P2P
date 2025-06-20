using Api.Models;
using BackendP2P.Models.Request;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackendP2P.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    //[Produces("application/json")]
    public class CommentController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly ITokenRevocationService _tokenRevocationService;

        public CommentController(ApplicationContext context, ITokenRevocationService tokenRevocationService)
        {
            _context = context;
            _tokenRevocationService = tokenRevocationService;
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel))]
        [HttpPost("{taskId}")]
        public async Task<IActionResult> CommentTask(Guid taskId, [FromBody] CommentCreateModel dto)
        {
            Console.WriteLine("Here?1");
            IActionResult? authResult = AuthenticateService();
            Console.WriteLine("Here?2");
            if (authResult != null) return authResult;
            Console.WriteLine("Here?3");
            Guid userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            Console.WriteLine("Here?4");
            try
            {
                TaskModel? task = await _context.Tasks.FirstOrDefaultAsync(u => u.Id == taskId);

                if (task == null)
                {
                    Console.WriteLine("Here?");
                    return NotFound("Task not found");
                }

                CommentModel comment = new CommentModel
                {
                    Id = Guid.NewGuid(),
                    Text = dto.Text,
                    AuthorId = userId,
                    CreateTime = DateTime.UtcNow,
                    Author = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId)
                };

                task.Comments ??= new List<CommentModel>();
                task.Comments.Add(comment);

                _context.Tasks.Update(task);
                await _context.Comments.AddAsync(comment);
                await _context.SaveChangesAsync();

                return Ok(new ResponseModel("Comment added to task"));
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
