
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

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CourseController : ControllerBase
{
    private readonly ITokenRevocationService _tokenRevocationService;

    private readonly ApplicationContext _context;
    private readonly ILogger<CourseController> _logger;

    public CourseController(
        ApplicationContext context,
        ILogger<CourseController> logger, ITokenRevocationService tokenRevocationService)
    {
        _context = context;
        _logger = logger;
        _tokenRevocationService = tokenRevocationService;

    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<CourseModel>> CreateCourse(
        [FromBody] CourseCreateModel model)

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

        try
        {
            // СДЕЛАТЬ ПРОВЕРКУ НА УНИКАЛЬНОСТЬ КОДА
            var teachersCode = GenerateRandomCode();
            var studentsCode = GenerateRandomCode();
            var course = new CourseModel
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Chapter = model.Chapter,
                Subject = model.Subject,
                Audience = model.Audience,
                TeachersCode = teachersCode,
                StudentsCode = studentsCode,
                CreateTime = DateTime.UtcNow
            };

            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return BadRequest(new { message = "Недействительный токен" });
            }

            var userId = Guid.Parse(userIdClaim.Value);

            var user = await _context.Users
             .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound(new { message = "Пользователь не найден" });
            }

            course.Users.Add(user);

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            
            UserCorse userCorse = new UserCorse
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Role = Role.Owner,
                CourseId = course.Id
            };

            _context.UsersCorses.Add(userCorse);
            await _context.SaveChangesAsync();


            return CreatedAtAction(
                nameof(GetCourse),
                new { id = course.Id },
                course);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании курса");
            return StatusCode(500, "Произошла ошибка при создании курса");
        }
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<CourseModel>> GetCourse(Guid id)
    {
        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            return BadRequest(new { message = "Недействительный токен" });
        }

        var userId = Guid.Parse(userIdClaim.Value);

         var object1 = _context.UsersCorses
            .Where(uc => uc.UserId == userId && uc.CourseId == id)
            .FirstOrDefault();
            
        if (object1 == null)
        {
            return NotFound();
        }
        var course = await _context.Courses
            .Include(c => c.Users)
            .Include(c => c.Tasks)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
        {
            return NotFound();
        }


        return Ok(course);
    }


    [HttpGet("{id}/Role")]
    [Authorize]
    public async Task<ActionResult<Role>> GetCorseRole(Guid id)
    {
        var course = await _context.Courses.FindAsync(id);

        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            return BadRequest(new { message = "Недействительный токен" });
        }

        var userId = Guid.Parse(userIdClaim.Value);

        var role = _context.UsersCorses
            .Where(uc => uc.UserId == userId && uc.CourseId == id)
            .Select(uc => uc.Role)
            .FirstOrDefault();
            
        if (course == null || role == null)
        {
            return NotFound();
        }


        return Ok(role);
    }

    [HttpPost("register")]
    [Authorize]
    public async Task<IActionResult> JoinCourseByCode(string code)
{
    // Получаем ID пользователя из токена
    var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
    
    if (userIdClaim == null)
    {
        return BadRequest(new { message = "Недействительный токен" });
    }

    if (!Guid.TryParse(userIdClaim.Value, out var userId))
    {
        return BadRequest(new { message = "Некорректный ID пользователя" });
    }

    var user = await _context.Users
        .FirstOrDefaultAsync(u => u.Id == userId);

    if (user == null)
    {
        return NotFound(new { message = "Пользователь не найден" });
    }

    var course = await _context.Courses
        .FirstOrDefaultAsync(c => c.StudentsCode == code || c.TeachersCode == code);

    if (course == null)
    {
        return NotFound(new { message = "Курс с таким кодом не найден" });
    }

    var userAlreadyInCourse = await _context.UsersCorses
        .AnyAsync(uc => uc.UserId == userId && uc.CourseId == course.Id);

    if (userAlreadyInCourse)
    {
        return Conflict(new { message = "Пользователь уже добавлен в этот курс" });
    }

    var userCourse = new UserCorse
    {
        Id = Guid.NewGuid(),
        UserId = userId,
        CourseId = course.Id,
        Role = course.StudentsCode == code ? Role.Student : Role.Teacher
    };

        course.Users.Add(user);

    _context.UsersCorses.Add(userCourse);
    await _context.SaveChangesAsync();

    return Ok(new { message = "Пользователь успешно добавлен в курс", courseId = course.Id });
}

    
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteCourse(Guid id)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return Unauthorized(new { status = "error", message = "Неавторизованный доступ" });
        }

        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (_tokenRevocationService.IsTokenRevoked(token))
        {
            return Unauthorized(new { status = "error", message = "Токен недействителен" });
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(new { status = "error", message = "Неверный идентификатор пользователя" });
        }
    
        var course = await _context.Courses
            .Include(c => c.Users)
            .Include(c => c.Tasks)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
        {
            return NotFound(new { message = "Курс не найден" });
        }

        // 2. Удаляем все связи пользователей с курсом
        if (course.Users != null && course.Users.Any())
        {
            foreach (var user in course.Users.ToList())
            {
                course.Users.Remove(user);
            }
        }

        // // 3. Удаляем все задачи курса
        // if (course.Tasks != null && course.Tasks.Any())
        // {
        //     _context.Tasks.RemoveRange(course.Tasks);
        // }


        _context.UsersCorses.RemoveRange();
        
        _context.Courses.Remove(course);
        
        var recordsToDelete = _context.UsersCorses.Where(uc => uc.CourseId == id).ToList();

        if (recordsToDelete.Any())
        {
            _context.UsersCorses.RemoveRange(recordsToDelete);
        }

        try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении курса");
                return StatusCode(500, new { status = "error", message = "Ошибка при удалении курса" });
            }
    }

 
    private string GenerateRandomCode(int length = 8)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}