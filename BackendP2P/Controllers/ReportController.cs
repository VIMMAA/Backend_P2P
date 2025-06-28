using BackendP2P.Models.Request;
using Domain.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims; 



[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
     private readonly ApplicationContext _context;
        private readonly ITokenRevocationService _tokenRevocationService;
        public ReportController(ApplicationContext context, ITokenRevocationService tokenRevocationService)
        {
            _context = context;
            _tokenRevocationService = tokenRevocationService;
        }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<ReportModel>>> GetReports()
    {
        var reports = await _context.Reports.ToListAsync();
        return Ok(reports);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<ReportModel>> GetReport(Guid id)
    {
        var report = await _context.Reports.FirstOrDefaultAsync(r => r.Id == id);
        if (report == null)
        {
            return NotFound();
        }
        return Ok(report);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ReportModel>> CreateReport([FromBody] ReportCreateModel createModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            return BadRequest(new { message = "Недействительный токен" });
        }

        var userId = Guid.Parse(userIdClaim.Value);

        var user = _context.Users.FirstOrDefault(u => u.Id == userId);

        var student = _context.Users.FirstOrDefault(u => u.Id == createModel.StudentId);

        var sol = _context.Solutions.FirstOrDefault(m => m.Id == createModel.SolutionId);
        if (sol == null)
        {
            return BadRequest(new { message = "Решение не найдено" });
        }
        
        var task = _context.MaterialWorks.FirstOrDefault(m => m.Id == sol.TaskId);
        
        if (task == null)
        {
            return BadRequest(new { message = "Задание не найдено" });
        }

        var reportCount = _context.Reports.Count();
        var newReport = new ReportModel
        {
            Id = Guid.NewGuid(),
            Numb = reportCount + 1,
            Description = createModel.Description,
            Theme = task.Name,
            StudentRep = student.FirstName + " " + student.LastName,
            Author = user.FirstName + " " + user.LastName,
            CreateTime = DateTime.UtcNow,
            Solution = sol,
            SolutionId = sol.Id
        };

         _context.Reports.Add(newReport);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetReport), new { id = newReport.Id }, newReport);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteReport(Guid id)
    {
        var report = await _context.Reports.FirstOrDefaultAsync(r => r.Id == id);
        if (report == null)
        {
            return NotFound();
        }

        _context.Reports.Remove(report);
        await _context.SaveChangesAsync();

        return NoContent();
    }

}