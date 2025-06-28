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
        [HttpPost("{taskId}/task")]
        public async Task<IActionResult> CommentTaskWork(Guid taskId, [FromBody] CommentCreateModel dto)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;
            Guid userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            try
            {
                TaskModel? task = await _context.MaterialWorks.Include(t => t.Comments).Include(t => t.Solutions).Include(t => t.CriteriaAssignments).ThenInclude(t => t.GradeModel).FirstOrDefaultAsync(t => t.Id == taskId);

                if (task == null)
                {
                    task = await _context.MaterialReads.Include(t => t.Comments).FirstOrDefaultAsync(t => t.Id == taskId);
                }

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

                if (task is MaterialWorkModel work)
                {
                    _context.MaterialWorks.Update(work);
                }
                else if (task is MaterialReadModel read)
                {
                    _context.MaterialReads.Update(read);
                }

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
        [HttpPut("{taskId}/task/{commentId}")]
        public async Task<IActionResult> EditCommentTaskWork(Guid taskId, Guid commentId, [FromBody] CommentCreateModel dto)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;
            Guid userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            try
            {
                TaskModel? task = await _context.MaterialWorks.Include(t => t.Comments).Include(t => t.Solutions).Include(t => t.CriteriaAssignments).ThenInclude(t => t.GradeModel).FirstOrDefaultAsync(t => t.Id == taskId);

                if (task == null)
                {
                    task = await _context.MaterialReads.Include(t => t.Comments).FirstOrDefaultAsync(t => t.Id == taskId);
                }

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

                if (task is MaterialWorkModel work)
                {
                    _context.MaterialWorks.Update(work);
                }
                else if (task is MaterialReadModel read)
                {
                    _context.MaterialReads.Update(read);
                }

                _context.Comments.Update(comment);

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
        [HttpDelete("{taskId}/task/{commentId}")]
        public async Task<IActionResult> CommentDeleteTaskWork(Guid commentId, Guid taskId)
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

                TaskModel? task = await _context.MaterialWorks.Include(t => t.Comments).Include(t => t.Solutions).Include(t => t.CriteriaAssignments).ThenInclude(t => t.GradeModel).FirstOrDefaultAsync(t => t.Id == taskId);

                if (task == null)
                {
                    task = await _context.MaterialReads.Include(t => t.Comments).FirstOrDefaultAsync(t => t.Id == taskId);
                }

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                task.Comments.Remove(comment);

                if (task is MaterialWorkModel work)
                {
                    _context.MaterialWorks.Update(work);
                }
                else if (task is MaterialReadModel read)
                {
                    _context.MaterialReads.Update(read);
                }

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

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CommentModel>))]
        [HttpGet("{taskId}/task/list")]
        public async Task<IActionResult> CommentGetTaskWorkList(Guid taskId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;
            Guid userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            try
            {
                TaskModel? task = await _context.MaterialWorks.Include(t => t.Comments).Include(t => t.Solutions).Include(t => t.CriteriaAssignments).ThenInclude(t => t.GradeModel).FirstOrDefaultAsync(t => t.Id == taskId);

                if (task == null)
                {
                    task = await _context.MaterialReads.Include(t => t.Comments).FirstOrDefaultAsync(t => t.Id == taskId);
                }

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