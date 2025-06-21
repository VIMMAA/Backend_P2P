//using Api.Models;
//using BackendP2P.Models.Request;
//using Domain.Enums;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Security.Claims;

//namespace BackendP2P.Controllers
//{
//    [Route("api/[controller]")]
//    [Authorize]
//    [ApiController]
//    [Produces("application/json")]
//    public class SolutionController : Controller
//    {
//        private readonly ApplicationContext _context;
//        private readonly ITokenRevocationService _tokenRevocationService;
//        public SolutionController(ApplicationContext context, ITokenRevocationService tokenRevocationService)
//        {
//            _context = context;
//            _tokenRevocationService = tokenRevocationService;
//        }

//        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel))]
//        [HttpPost("{courseId}")]
//        public async Task<IActionResult> CreateSolution([FromBody] SolutionCreateModel model, Guid courseId)
//        {
//            IActionResult? authResult = AuthenticateService();
//            if (authResult != null) return authResult;

//            IActionResult? httpResult = IsForbid(true, courseId);
//            if (httpResult != null)
//            {
//                return httpResult;
//            }
//            try
//            {
//                var userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

//                SolutionModel solution = new SolutionModel
//                {
//                    Id = Guid.NewGuid(),
//                    StudentId = userId,
//                    Student = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId),
//                    SubmissionTime = DateTime.UtcNow,
//                    AttachmentPath = model.AttachmentPath,
//                    Content = model.Content,
//                    Task = 
//                };
//            }
//            catch (Exception ex)
//            {
//                Console.Error.WriteLine($"\nERROR\n{ex}");

//                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
//            }
//        }

//        private IActionResult? AuthenticateService()
//        {
//            if (!User.Identity.IsAuthenticated)
//            {
//                return Unauthorized(new { status = "error", message = "Неавторизованный доступ" });
//            }

//            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

//            if (_tokenRevocationService.IsTokenRevoked(token))
//            {
//                return Unauthorized(new { status = "error", message = "Неавторизованный доступ" });
//            }

//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

//            if (userIdClaim == null)
//            {
//                return BadRequest(new { message = "Invalid token" });
//            }

//            return null;
//        }
//        private IActionResult? IsForbid(bool isStudent, Guid courseId)
//        {
//            var userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

//            UserCorse? userCorse = _context.UsersCorses.FirstOrDefault(x => x.UserId == userId && x.CourseId == courseId);

//            if (userCorse == null)
//            {
//                return Forbid();
//            }


//            if (!isStudent)
//            {
//                Role? role = userCorse.Role;
//                if (role == Role.Student || role == null)
//                {
//                    return Forbid();
//                }
//            }

//            return null;
//        }
//    }
//}
