
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
using BackendP2P.Models.Request;

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
        public async Task<IActionResult> SubmitCheck(Guid id, [FromBody] SolutionForCheckModel submission)
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

                int totalScore = submission.Assements
                .Where(a => a.Score.HasValue) 
                .Sum(a => a.Score.Value);

                if (userCourse.Role == Role.Teacher || userCourse.Role == Role.Owner)
                {
                    GradeModel grade = new GradeModel
                    {
                        Id = Guid.NewGuid(),
                        TeacherId = currentUserId,
                        Score = totalScore,
                        StudentId = checkTask.Solution.StudentId,
                        TaskId = checkTask.Solution.TaskId,
                        Remark = submission.Comment,
                    };
                    await _context.Grades.AddAsync(grade);
                }

                await _context.SaveChangesAsync();

                return Ok("Результат проверки успешно сохранен");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Произошла внутренняя ошибка сервера");
            }
        }


       [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        [HttpGet("solution/{solutionId}")]

        public async Task<IActionResult> GetAssessmentsBySolutionId(Guid solutionId)
        {
            var solution = await _context.Solutions
                .Include(s => s.Task)
                .ThenInclude(t => t.Course)
                .FirstOrDefaultAsync(s => s.Id == solutionId);

            if (solution == null)
            {
                return NotFound("Решение не найдено");
            }

            var checks = await _context.SolutionForChecks
                .Include(s => s.Assements)
                .Where(s => s.SolutionId == solutionId)
                .ToListAsync();

            if (!checks.Any())
            {
                return NotFound("Проверки для указанного решения не найдены");
            }

            var authorIds = checks.Select(c => c.AuthortId).Distinct();

            var teacherIds = await _context.UsersCorses
                .Where(uc => uc.CourseId == solution.Task.CourseId &&
                           (uc.Role == Role.Teacher || uc.Role == Role.Owner) &&
                            authorIds.Contains(uc.UserId))
                .Select(uc => uc.UserId)
                .ToListAsync();

            var result = new AssessmentList
            {
                SolutionForCheckModels = checks,
                TeacherAssessment = checks.FirstOrDefault(c => teacherIds.Contains(c.AuthortId)),
                Grade = await _context.Grades
                    .FirstOrDefaultAsync(g => g.TaskId == solution.TaskId &&
                                            g.StudentId == solution.StudentId)
            };

            return Ok(result);
        }
    
        private Guid GetCurrentUserId()
        {
            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = Guid.Parse(userIdClaim.Value);
            return userId;
        }

    }
}