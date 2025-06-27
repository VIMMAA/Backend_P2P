
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Api.Models;
using System.Security.Claims; 
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Domain.Entities;
using Domain.Enums;


namespace MyApi.MapControllers
{


    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]

    public class CheckAssignmentsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        private readonly ITokenRevocationService _tokenRevocationService;


        public CheckAssignmentsController(
        ApplicationContext context, ITokenRevocationService tokenRevocationService)
        {
            _context = context;
            _tokenRevocationService = tokenRevocationService;

        }

    [HttpGet("List")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<List<PackageCheckModel>>>> GetMyChecks()
    {
        var currentUserId = GetCurrentUserId();

        var checks = await _context.PackageChecks
            .Where(p => p.SolutionForCheckTasks.Any(s => s.AuthortId == currentUserId))
            .Include(p => p.SolutionForCheckTasks)
            .ThenInclude(s => s.Assements)
            .OrderByDescending(p => p.Deadline)
            .ToListAsync();

            foreach (var check in checks)
            {
                var List = new List<SolutionForCheckModel>();

                foreach (var sol in check.SolutionForCheckTasks)
                {
                    if (sol.AuthortId == currentUserId)
                    {
                        List.Add(sol);
                    }
                }
                check.SolutionForCheckTasks = List;
            }

        return Ok(checks);
    }

        [HttpGet("{id}")]
        public async Task<ActionResult<SolutionForCheckModel>> GetCheckTask(Guid id)
        {
            var currentUserId = GetCurrentUserId();

            var checkTask = await _context.SolutionForChecks
            .Include(s => s.Assements)
            .FirstOrDefaultAsync(s => s.Id == id && s.AuthortId == currentUserId);

            if (checkTask == null)
            {
                return NotFound("Задание на проверку не найдено или у вас нет к нему доступа");
            }

            return Ok(checkTask);

        }

        [HttpPost("{id}/submit")]
        [Authorize]
        public async Task<IActionResult> SubmitCheck(
            Guid id,
            [FromBody] SolutionForCheckModel submission)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                var checkTask = await _context.SolutionForChecks
                    .Include(sc => sc.Solution)
                        .ThenInclude(s => s.Task)
                    .FirstOrDefaultAsync(s => s.Id == id && s.AuthortId == currentUserId);

                if (checkTask == null)
                {
                    return NotFound("Задание на проверку не найдено или у вас нет к нему доступа");
                }

                var courseId = checkTask.Solution.Task.CourseId;
                var userCourse = await _context.UsersCorses
                    .FirstOrDefaultAsync(uc => uc.CourseId == courseId && uc.UserId == currentUserId);

                if (userCourse == null)
                {
                    return Forbid("У вас нет доступа к этому курсу");
                }

                var packageCheck = await _context.PackageChecks
                    .FirstOrDefaultAsync(p => p.SolutionForCheckTasks.Any(s => s.Id == id));

                if (packageCheck == null)
                {
                    return NotFound("Пакет проверки не найден");
                }

                if (userCourse.Role == Role.Student && DateTime.UtcNow > packageCheck.Deadline)
                {
                    return BadRequest("Время для проверки истекло");
                }

                checkTask.Comment = submission.Comment;
                checkTask.Assements = submission.Assements;
                checkTask.IsChecked = true;
                checkTask.DueTime = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok("Результат проверки успешно сохранен");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Произошла внутренняя ошибка сервера");
            }
        }
        private Guid GetCurrentUserId()
        {
            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = Guid.Parse(userIdClaim.Value);
            return userId;
        }

    }
}