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
        [HttpPost("{taskId}/materialWork")]
        public async Task<IActionResult> CommentTaskWork(Guid taskId, [FromBody] CommentCreateModel dto)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;
            Guid userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            try
            {
                MaterialWorkModel? task = await _context.MaterialWorks.Include(t => t.Comments).Include(t => t.Solutions).Include(t => t.CriteriaAssignments).ThenInclude(t => t.GradeModel).FirstOrDefaultAsync(t => t.Id == taskId);

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

                _context.MaterialWorks.Update(task);
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
        [HttpPut("{taskId}/materialWork/{commentId}")]
        public async Task<IActionResult> EditCommentTaskWork(Guid taskId, Guid commentId, [FromBody] CommentCreateModel dto)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;
            Guid userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            try
            {
                MaterialWorkModel? task = await _context.MaterialWorks.Include(t => t.Comments).Include(t => t.Solutions).Include(t => t.CriteriaAssignments).ThenInclude(t => t.GradeModel).FirstOrDefaultAsync(t => t.Id == taskId);

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
                _context.MaterialWorks.Update(task);
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
        [HttpDelete("{taskId}/materialWork/{commentId}")]
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
                MaterialWorkModel? task = await _context.MaterialWorks.Include(t => t.Comments).Include(t => t.Solutions).Include(t => t.CriteriaAssignments).ThenInclude(t => t.GradeModel).FirstOrDefaultAsync(t => t.Id == taskId);

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                task.Comments.Remove(comment);

                _context.MaterialWorks.Update(task);
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
        [HttpGet("{taskId}/materialWork/{commentId}")]
        public async Task<IActionResult> CommentGetTaskWork(Guid commentId, Guid taskId)
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

                MaterialWorkModel? task = await _context.MaterialWorks.Include(t => t.Comments).Include(t => t.Solutions).Include(t => t.CriteriaAssignments).ThenInclude(t => t.GradeModel).FirstOrDefaultAsync(t => t.Id == taskId);
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
        
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CommentModel>))]
        [HttpGet("{taskId}/materialWork/list")]
        public async Task<IActionResult> CommentGetTaskWorkList(Guid taskId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;
            Guid userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            try
            {
                MaterialWorkModel? task = await _context.MaterialWorks.Include(t => t.Comments).Include(t => t.Solutions).Include(t => t.CriteriaAssignments).ThenInclude(t => t.GradeModel).FirstOrDefaultAsync(t => t.Id == taskId);

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











        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel))]
        [HttpPost("{taskId}/materialRead")]
        public async Task<IActionResult> CommentTaskRead(Guid taskId, [FromBody] CommentCreateModel dto)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;
            Guid userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            try
            {
                MaterialReadModel? task = await _context.MaterialReads.Include(t => t.Comments).FirstOrDefaultAsync(t => t.Id == taskId);

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

                _context.MaterialReads.Update(task);
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
        [HttpPut("{taskId}/materialRead/{commentId}")]
        public async Task<IActionResult> EditCommentTaskRead(Guid taskId, Guid commentId, [FromBody] CommentCreateModel dto)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;
            Guid userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            try
            {
                MaterialReadModel? task = await _context.MaterialReads.Include(t => t.Comments).FirstOrDefaultAsync(t => t.Id == taskId);

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
                _context.MaterialReads.Update(task);
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
        [HttpDelete("{taskId}/materialRead/{commentId}")]
        public async Task<IActionResult> CommentDeleteTaskRead(Guid commentId, Guid taskId)
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
                MaterialReadModel? task = await _context.MaterialReads.Include(t => t.Comments).FirstOrDefaultAsync(t => t.Id == taskId);

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                task.Comments.Remove(comment);

                _context.MaterialReads.Update(task);
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
        [HttpGet("{taskId}/materialRead/{commentId}")]
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

                MaterialReadModel? task = await _context.MaterialReads.Include(t => t.Comments).FirstOrDefaultAsync(t => t.Id == taskId);

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

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CommentModel>))]
        [HttpGet("{taskId}/materialRead/list")]
        public async Task<IActionResult> CommentGetTaskReadList(Guid taskId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;
            Guid userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            try
            {
                MaterialReadModel? task = await _context.MaterialReads.Include(t => t.Comments).FirstOrDefaultAsync(t => t.Id == taskId);

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
