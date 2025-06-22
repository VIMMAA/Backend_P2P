using Api.Models;
using BackendP2P.Models.Request;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ApiB.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    public class TaskController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly ITokenRevocationService _tokenRevocationService;

        public TaskController(ApplicationContext context, ITokenRevocationService tokenRevocationService)
        {
            _context = context;
            _tokenRevocationService = tokenRevocationService;
        }
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TaskCreatedModel))]
        [HttpPost("{courseId}")]
        public async Task<IActionResult> CreateTask([FromBody] TaskCreateModel model, Guid courseId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;

            IActionResult? httpResult = IsForbid(false, courseId);
            if (httpResult != null)
            {
                return httpResult;
            }

            try
            {
                var userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                TaskModel task = new TaskModel
                {
                    Id = Guid.NewGuid(),
                    AuthorId = userId,
                    Author = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId),
                    Students = model.Students?.ToList() ?? new List<Guid>(),
                    CourseId = courseId,
                    Course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId),
                    Name = model.Name,
                    Topic = model.Topic,
                    CreateTime = DateTime.UtcNow,
                    Deadline = model.Deadline,
                    Check = model.isP2P ? Check.P2P : Check.TeacherOnly,
                    Comments = new List<CommentModel>(),
                    Solutions = new List<SolutionModel>()
                };

                if (task.Deadline < DateTime.UtcNow)
                {
                    return BadRequest("Deadline is outdated");
                }

                foreach (var studentId in task.Students)
                {
                    if (task.Students.Count != task.Students.Distinct().Count())
                        return BadRequest(new { message = "Duplicate students in the list." });
                    if (!await _context.UsersCorses.AnyAsync(s => s.UserId == studentId && task.CourseId == s.CourseId && s.Role == Role.Student))
                        return BadRequest(new { message = "At least one of the students not found on this course" });

                }

                TaskCreatedModel answer = new TaskCreatedModel
                {
                    Id = task.Id,
                    AuthorId = task.AuthorId,
                    Students = task.Students,
                    CourseId = task.CourseId,
                    Name = task.Name,
                    Topic = task.Topic,
                    CreateTime = task.CreateTime,
                    Deadline = task.Deadline,
                    Check = task.Check,
                    Comments = new List<CommentModel>(),
                    Solutions = new List<SolutionModel>(),
                    Grades = new List<GradeModel>()
                };

                await _context.Tasks.AddAsync(task);
                await _context.SaveChangesAsync();

                return Ok(answer);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TaskCreatedModel))]
        [HttpGet("{taskId}")]//check for forbid error
        public async Task<IActionResult> GetTask(Guid taskId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;

            try
            {
                TaskModel task = await _context.Tasks.Include(t => t.Comments).Include(t => t.Solutions).FirstOrDefaultAsync(t => t.Id == taskId);

                IActionResult? httpResult = IsForbid(true, task.CourseId);
                if (httpResult != null)
                {
                    return httpResult;
                }

                if (task == null)
                    return NotFound(new { message = "Task not found" });

                TaskCreatedModel answer = new TaskCreatedModel
                {
                    Id = task.Id,
                    AuthorId = task.AuthorId,
                    Students = task.Students,
                    CourseId = task.CourseId,
                    Name = task.Name,
                    Topic = task.Topic,
                    CreateTime = task.CreateTime,
                    Deadline = task.Deadline,
                    Check = task.Check,
                    Comments = task.Comments.Select(c => new CommentModel
                    {
                        Id = c.Id,
                        Text = c.Text,
                        AuthorId = c.AuthorId,
                        CreateTime = c.CreateTime
                    }).ToList(),
                    Solutions = task.Solutions.Select(s => new SolutionModel { 
                        Id = s.Id,
                        SubmissionTime = s.SubmissionTime,
                        StudentId = s.StudentId,
                        Content = s.Content,
                        AttachmentPath = s.AttachmentPath,
                        TaskId = s.TaskId,
                    }).ToList(),
                    MaterialReadId = task.MaterialReadId,
                    MaterialWorkId = task.MaterialWorkId,
                };

                return Ok(answer);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel))]
        [HttpPut("{taskId}")]
        public async Task<IActionResult> EditTask(Guid taskId, [FromBody] TaskEditModel dto)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;

            try
            {
                var userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                TaskModel task = await _context.Tasks.Include(t => t.Comments).Include(t => t.Solutions).FirstOrDefaultAsync(t => t.Id == taskId);

                IActionResult? httpResult = IsForbid(false, task.CourseId);
                if (httpResult != null)
                {
                    return httpResult;
                }

                if (task == null)
                    return NotFound(new { message = "Task not found" });

                task.Name = dto.Name;
                task.Students = dto.Students;
                task.Topic = dto.Topic;
                task.Deadline = dto.Deadline;

                if (task.Deadline < DateTime.UtcNow)
                {
                    return BadRequest("Deadline is outdated");
                }

                foreach (var studentId in task.Students)
                {
                    if (task.Students.Count != task.Students.Distinct().Count())
                        return BadRequest(new { message = "Duplicate students in the list." });
                    if (!await _context.UsersCorses.AnyAsync(s => s.UserId == studentId && task.CourseId == s.CourseId && s.Role == Role.Student))
                        return BadRequest(new { message = "One of the students not found in StudentCorse" });

                }

                _context.Tasks.Update(task);
                await _context.SaveChangesAsync();

                return Ok(new ResponseModel("Task updated"));
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel))]
        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask(Guid taskId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;

            try
            {

                TaskModel? task = await _context.Tasks.FirstOrDefaultAsync(u => u.Id == taskId);

                IActionResult? httpResult = IsForbid(false, task.CourseId);
                if (httpResult != null)
                {
                    return httpResult;
                }

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();

                return Ok(new ResponseModel("Task deleted"));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MaterialReadModel))]
        [HttpPost("{taskId}/readMaterial")]
        public async Task<IActionResult> CreateReadTask(Guid taskId, [FromBody] TaskReadCreateDto taskRead)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;

            try
            {
                TaskModel? task = await _context.Tasks.FirstOrDefaultAsync(u => u.Id == taskId);

                IActionResult? httpResult = IsForbid(false, task.CourseId);
                if (httpResult != null)
                {
                    return httpResult;
                }

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                if (await _context.MaterialReads.AnyAsync(u => u.TaskId == taskId))
                {
                    return BadRequest("Task already have read material");
                }

                MaterialReadModel updateRead = new MaterialReadModel
                {
                    Id = Guid.NewGuid(),
                    TaskId = taskId,
                    Task = task,
                    Content = taskRead.Content
                };
                
                task.MaterialReadId = updateRead.Id;
                task.MaterialReadModel = updateRead;

                _context.Tasks.Update(task);
                await _context.MaterialReads.AddAsync(updateRead);
                await _context.SaveChangesAsync();

                return Ok(updateRead);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel))]
        [HttpDelete("{taskId}/readMaterial")]
        public async Task<IActionResult> DeleteReadTask(Guid taskId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;
            try
            {
                TaskModel? task = await _context.Tasks.FirstOrDefaultAsync(u => u.Id == taskId);

                IActionResult? httpResult = IsForbid(false, task.CourseId);
                if (httpResult != null)
                {
                    return httpResult;
                }

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                MaterialReadModel? readModel = await _context.MaterialReads.FirstOrDefaultAsync(u => u.TaskId == taskId);

                if (readModel == null)
                {
                    return BadRequest("Task doesn't have any read material. Deleting is impossible.");
                }

                task.MaterialReadId = null;
                task.MaterialReadModel = null;

                _context.MaterialReads.Remove(readModel);
                _context.Tasks.Update(task);
                await _context.SaveChangesAsync();

                return Ok(new ResponseModel($"Read material {readModel.Id} deleted"));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel))]
        [HttpPatch("{taskId}/readMaterial/change")]
        public async Task<IActionResult> ReplaceReadTask(Guid taskId, [FromBody] TaskReadCreateDto taskRead)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;

            try
            {
                TaskModel? task = await _context.Tasks.FirstOrDefaultAsync(u => u.Id == taskId);

                IActionResult? httpResult = IsForbid(false, task.CourseId);
                if (httpResult != null)
                {
                    return httpResult;
                }

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                MaterialReadModel? readModel = await _context.MaterialReads.FirstOrDefaultAsync(u => u.TaskId == taskId);

                if (readModel == null)
                {
                    return BadRequest("Task doesn't have any read material. Replacing is impossible.");
                }

                _context.MaterialReads.Remove(readModel);

                MaterialReadModel updateRead = new MaterialReadModel
                {
                    Id = Guid.NewGuid(),
                    TaskId = taskId,
                    Task = task,
                    Content = taskRead.Content
                };

                await _context.MaterialReads.AddAsync(updateRead);

                task.MaterialReadId = updateRead.Id;
                task.MaterialReadModel = updateRead;

                _context.Tasks.Update(task);
                await _context.SaveChangesAsync();

                return Ok(new ResponseModel($"Read material {readModel.Id} was replaced by {updateRead.Id}"));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MaterialReadModel))]
        [HttpGet("{taskId}/readMaterial")]
        public async Task<IActionResult> GetReadTask(Guid taskId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;

            try
            {
                TaskModel? task = await _context.Tasks.FirstOrDefaultAsync(u => u.Id == taskId);

                IActionResult? httpResult = IsForbid(true, task.CourseId);
                if (httpResult != null)
                {
                    return httpResult;
                }

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                MaterialReadModel? readModel = await _context.MaterialReads.FirstOrDefaultAsync(u => u.TaskId == taskId);

                if (readModel == null)
                {
                    return BadRequest("Task doesn't have any read material.");
                }

                return Ok(readModel);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MaterialWorkModel))]
        [HttpPost("{taskId}/workMaterial")]
        public async Task<IActionResult> CreateWorkTask(Guid taskId, [FromBody] CombinedTaskWorkAndCriteriaModels CombinedDto)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;
            
            try
            {
                TaskModel? task = await _context.Tasks.FirstOrDefaultAsync(u => u.Id == taskId);

                IActionResult? httpResult = IsForbid(false, task.CourseId);
                if (httpResult != null)
                {
                    return httpResult;
                }

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                if (await _context.MaterialWorks.AnyAsync(u => u.TaskId == taskId))
                {
                    return BadRequest("Task already have work material");
                }


                MaterialWorkModel updateWork = new MaterialWorkModel
                {
                    Id = Guid.NewGuid(),
                    TaskId = taskId,
                    Task = task,
                    Score = CombinedDto.MaterialTaskWork.Score,
                    Deadline = task.Deadline,
                    Instructions = CombinedDto.MaterialTaskWork.Instructions
                };

                if (task.Deadline < DateTime.UtcNow)
                {
                    return BadRequest("Deadline is outdated");
                }

                CriteriaAssignment criteria = new CriteriaAssignment
                {
                    Id = Guid.NewGuid(),
                    Title = CombinedDto.CriteriaAssignment.Title,
                    Conditions = CombinedDto.CriteriaAssignment.Conditions,
                    CountScore = CombinedDto.CriteriaAssignment.CountScore,
                    Level = CombinedDto.CriteriaAssignment.Level,
                    MaterialWorkModelId = updateWork.Id,
                    MaterialWorkModel = updateWork
                };
                updateWork.CriteriaAssignments ??= new List<CriteriaAssignment>();
                updateWork.CriteriaAssignments.Add(criteria);

                task.MaterialWorkId = updateWork.Id;
                task.MaterialWorkModel = updateWork;

                _context.Tasks.Update(task);
                await _context.CriteriaAssignments.AddAsync(criteria);
                await _context.MaterialWorks.AddAsync(updateWork);
                await _context.SaveChangesAsync();

                return Ok(updateWork);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel))]
        [HttpDelete("{taskId}/workMaterial")]
        public async Task<IActionResult> DeleteWorkTask(Guid taskId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;

            try
            {
                TaskModel? task = await _context.Tasks.FirstOrDefaultAsync(u => u.Id == taskId);

                IActionResult? httpResult = IsForbid(false, task.CourseId);
                if (httpResult != null)
                {
                    return httpResult;
                }

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                MaterialWorkModel materialWork = await _context.MaterialWorks.FirstOrDefaultAsync(u => u.TaskId == taskId);

                if (materialWork == null)
                {
                    return NotFound("Material work not found");
                }

                _context.MaterialWorks.Remove(materialWork);

                task.MaterialWorkId = null;
                task.MaterialWorkModel = null;

                _context.Tasks.Update(task);
                await _context.SaveChangesAsync();

                return Ok(new ResponseModel("Work task deleted"));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MaterialWorkModel))]
        [HttpGet("{taskId}/workMaterial")]
        public async Task<IActionResult> GetWorkTask(Guid taskId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;

            try
            {
                TaskModel? task = await _context.Tasks.FirstOrDefaultAsync(u => u.Id == taskId);

                IActionResult? httpResult = IsForbid(true, task.CourseId);
                if (httpResult != null)
                {
                    return httpResult;
                }

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                MaterialWorkModel? materialWork = await _context.MaterialWorks.Include(t => t.CriteriaAssignments).FirstOrDefaultAsync(u => u.TaskId == taskId);

                if (materialWork == null)
                {
                    return BadRequest("Task doesn't have any work material.");
                }

                return Ok(materialWork);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel))]
        [HttpPut("{taskId}/workMaterial")]
        public async Task<IActionResult> PutWorkTask(Guid taskId, [FromBody] TaskWorkCreateModel dto)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;

            try
            {
                TaskModel? task = await _context.Tasks.FirstOrDefaultAsync(u => u.Id == taskId);

                IActionResult? httpResult = IsForbid(false, task.CourseId);
                if (httpResult != null)
                {
                    return httpResult;
                }

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                MaterialWorkModel? materialWork = await _context.MaterialWorks.FirstOrDefaultAsync(u => u.TaskId == taskId);

                if (materialWork == null)
                {
                    return BadRequest("Task doesn't have any work material.");
                }

                if (task.Deadline < DateTime.UtcNow)
                {
                    return BadRequest("Deadline is outdated");
                }

                materialWork.Instructions = dto.Instructions;
                materialWork.Score = dto.Score;
                //criterias mb
                _context.MaterialWorks.Update(materialWork);
                await _context.SaveChangesAsync();

                return Ok(new ResponseModel("Work material is updated"));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel))]
        [HttpPost("{taskId}/workMaterial/Criteria")]
        public async Task<IActionResult> CreateCriteria(Guid taskId, [FromBody] CriteriaAssignmentCreateModel dto)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;

            try
            {
                TaskModel? task = await _context.Tasks.FirstOrDefaultAsync(u => u.Id == taskId);

                IActionResult? httpResult = IsForbid(false, task.CourseId);
                if (httpResult != null)
                {
                    return httpResult;
                }

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                MaterialWorkModel? materialWork = await _context.MaterialWorks.FirstOrDefaultAsync(u => u.TaskId == taskId);

                if (materialWork == null)
                {
                    return BadRequest("Task doesn't have any work material.");
                }

                CriteriaAssignment criteria = new CriteriaAssignment
                {
                    Id = Guid.NewGuid(),
                    Conditions = dto.Conditions,
                    CountScore = dto.CountScore,
                    Level = dto.Level,
                    Title = dto.Title,
                    MaterialWorkModelId = materialWork.Id,
                    MaterialWorkModel = materialWork
                };

                if (task.Deadline < DateTime.UtcNow)
                {
                    return BadRequest("Deadline is outdated");
                }

                materialWork.CriteriaAssignments ??= new List<CriteriaAssignment>();
                materialWork.CriteriaAssignments.Add(criteria);

                await _context.CriteriaAssignments.AddAsync(criteria);
                _context.MaterialWorks.Update(materialWork);
                await _context.SaveChangesAsync();

                return Ok(new ResponseModel("Criteria added"));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(ResponseModel))]
        [HttpDelete("{taskId}/workMaterial/Criteria/{criteriaId}")]
        public async Task<IActionResult> DeleteCriteria(Guid taskId, Guid criteriaId)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;
            try
            {
                TaskModel? task = await _context.Tasks.FirstOrDefaultAsync(u => u.Id == taskId);

                IActionResult? httpResult = IsForbid(false, task.CourseId);
                if (httpResult != null)
                {
                    return httpResult;
                }

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                MaterialWorkModel? materialWork = await _context.MaterialWorks.FirstOrDefaultAsync(u => u.TaskId == taskId);

                if (materialWork == null)
                {
                    return BadRequest("Task doesn't have any work material.");
                }

                CriteriaAssignment? criteria = await _context.CriteriaAssignments.FirstOrDefaultAsync(u => criteriaId == u.Id);

                if (criteria == null)
                {
                    return NotFound("Criteria not found");
                }

                materialWork.CriteriaAssignments.Remove(criteria);
                _context.CriteriaAssignments.Remove(criteria);
                _context.MaterialWorks.Update(materialWork);
                await _context.SaveChangesAsync();
                return Ok(new ResponseModel("Criteria deleted"));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nERROR\n{ex}");

                return StatusCode(500, new { Status = "error", Message = "SWAGA" });
            }
        }
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel))]
        [HttpPut("{taskId}/workMaterial/Criteria/{criteriaId}")]
        public async Task<IActionResult> EditCriteria(Guid taskId, Guid criteriaId, [FromBody] CriteriaAssignmentCreateModel dto)
        {
            IActionResult? authResult = AuthenticateService();
            if (authResult != null) return authResult;
            try
            {
                TaskModel? task = await _context.Tasks.FirstOrDefaultAsync(u => u.Id == taskId);

                IActionResult? httpResult = IsForbid(false, task.CourseId);
                if (httpResult != null)
                {
                    return httpResult;
                }

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                if (task.Deadline < DateTime.UtcNow)
                {
                    return BadRequest("Deadline is outdated");
                }

                MaterialWorkModel? materialWork = await _context.MaterialWorks.FirstOrDefaultAsync(u => u.TaskId == taskId);

                if (materialWork == null)
                {
                    return BadRequest("Task doesn't have any work material.");
                }

                CriteriaAssignment? criteria = await _context.CriteriaAssignments.FirstOrDefaultAsync(u => criteriaId == u.Id);

                if (criteria == null)
                {
                    return NotFound("Criteria not found");
                }

                criteria.Title = dto.Title;
                criteria.Level = dto.Level;
                criteria.Conditions = dto.Conditions;
                criteria.CountScore = dto.CountScore;

                _context.CriteriaAssignments.Update(criteria);
                await _context.SaveChangesAsync();

                return Ok(new ResponseModel("Criteria updated"));
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
        private IActionResult? IsForbid(bool isStudent, Guid courseId)
        {
            var userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            UserCorse? userCorse =  _context.UsersCorses.FirstOrDefault(x => x.UserId == userId && x.CourseId == courseId);

            if (userCorse == null)
            {
                return Forbid();
            }

            
            if (!isStudent)
            {
                Role? role = userCorse.Role;
                if (role == Role.Student || role == null)
                {
                    return Forbid();
                }
            }

            return null;
        }
    }
}
