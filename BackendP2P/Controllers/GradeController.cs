
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

    public class GradeController : ControllerBase
    {
        private readonly ApplicationContext _context;

        private readonly ITokenRevocationService _tokenRevocationService;


        public GradeController(
        ApplicationContext context, ITokenRevocationService tokenRevocationService)
        {
            _context = context;
            _tokenRevocationService = tokenRevocationService;

        }

        [HttpGet("{courseId}/user/{userId} ")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<List<GradeShortModel>>>> GetMyChecks(Guid courseId, Guid userId)
        {
            var tasks = await _context.MaterialWorks
                .Where(t => t.CourseId == courseId)
                .ToListAsync();

            if (tasks == null)
            {
                return NotFound("На курсе нет заданий");
            }
            var list = new List<GradeShortModel>();

            foreach (var task in tasks)
            {
                var solution = await _context.Solutions
                    .FirstOrDefaultAsync(s => task.Id == s.TaskId && userId == s.StudentId);



                var grade = await _context.Grades.
                    FirstOrDefaultAsync(g => g.TaskId == task.Id && userId == g.StudentId);

                var model = new GradeShortModel
                {
                    Score = grade.Score,
                    SolutionId = solution.Id,
                    GradeId = grade.Id,
                    TaskName = task.Name
                };

                list.Add(model);


            }


            return Ok(list);
        }

        [HttpPut("{gradeId}")]
        [Authorize]
        public async Task<IActionResult> UpdateGrade(Guid gradeId, [FromBody] UpdateGradeRequest request)
        {
            // Проверяем, существует ли оценка
            var grade = await _context.Grades.FindAsync(gradeId);
            if (grade == null)
            {
                return NotFound("Оценка не найдена");
            }

            var currentUserId = GetCurrentUserId();
           

            // Обновляем оценку
            grade.Score = request.Score;
            grade.Remark = request.Comment;
            grade.TeacherId = currentUserId;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(grade);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GradeExists(gradeId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool GradeExists(Guid gradeId)
        {
            return _context.Grades.Any(e => e.Id == gradeId);
        }



        private Guid GetCurrentUserId()
        {
            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = Guid.Parse(userIdClaim.Value);
            return userId;
        }

    }
    
    public class UpdateGradeRequest
{
    public int Score { get; set; }
    public string? Comment { get; set; }
}
}