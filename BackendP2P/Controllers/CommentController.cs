using Api.Models;
using BackendP2P.Models.Request;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using System.Security.Claims;

namespace BackendP2P.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
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
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;
            Guid userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            try
            {
                TaskModel? task = await _context.Tasks.FirstOrDefaultAsync(u => u.Id == taskId);

                if (task == null)
                {
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel))]
        [HttpPut("{taskId}/{commentId}")]
        public async Task<IActionResult> EditCommentTask(Guid taskId, Guid commentId, [FromBody] CommentCreateModel dto)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;
            Guid userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            try
            {
                TaskModel? task = await _context.Tasks.FirstOrDefaultAsync(u => u.Id == taskId);

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                CommentModel? comment = await _context.Comments.FirstOrDefaultAsync(u => u.Id == commentId && u.AuthorId == userId);

                if (comment == null)
                {
                    return NotFound("Comment not found");
                }

                comment.Text = dto.Text;


                _context.Comments.Update(comment);
                _context.Tasks.Update(task);
                await _context.SaveChangesAsync();

                return Ok(new ResponseModel("Comment updated in task"));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel))]
        [HttpDelete("{taskId}/{commentId}")]
        public async Task<IActionResult> CommentDeleteTask(Guid commentId, Guid taskId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;
            Guid userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            try
            {
                CommentModel? comment = await _context.Comments.FirstOrDefaultAsync(u => u.Id == commentId && u.AuthorId == userId);

                if (comment == null)
                {
                    return NotFound("Comment not found");
                }

                TaskModel? task = await _context.Tasks.Include(u => u.Comments).FirstOrDefaultAsync(u => u.Id == taskId);
                if (task == null)
                {
                    return NotFound("Task not found");
                }

                task.Comments.Remove(comment);

                _context.Tasks.Update(task);
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();

                return Ok(new ResponseModel("Comment deleted from task"));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CommentModel))]
        [HttpGet("{taskId}/{commentId}")]
        public async Task<IActionResult> CommentGetTask(Guid commentId, Guid taskId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;
            Guid userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            try
            {
                CommentModel? comment = await _context.Comments.FirstOrDefaultAsync(u => u.Id == commentId);

                if (comment == null)
                {
                    return NotFound("Comment not found");
                }

                TaskModel? task = await _context.Tasks.Include(u => u.Comments).FirstOrDefaultAsync(u => u.Id == taskId);
                if (task == null)
                {
                    return NotFound("Task not found");
                }

                return Ok(comment);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CommentModel))]
        [HttpGet("{taskId}/list")]
        public async Task<IActionResult> CommentGetTaskList(Guid commentId, Guid taskId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;
            Guid userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            try
            {
                TaskModel? task = await _context.Tasks.Include(u => u.Comments).FirstOrDefaultAsync(u => u.Id == taskId);
                if (task == null)
                {
                    return NotFound("Task not found");
                }

                return Ok(task.Comments);
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
