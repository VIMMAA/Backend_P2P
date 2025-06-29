using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Api.Models;
using Domain.Enums;

public class SolutionDistributionService
{
    private readonly ApplicationContext _context;

    public SolutionDistributionService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task DistributeSolutionsAfterDeadline()
    {

        var expiredMaterialWorks = await _context.MaterialWorks
        .Where(mw => mw.Deadline < DateTime.UtcNow)
        .Include(mw => mw.CriteriaAssignments)
        .ToListAsync();


        foreach (var task in expiredMaterialWorks)
        {
            if (!task.SolutionsDistributed)
            {
                await DistributeSolutionsForTask(task);
            }
        }

        await _context.SaveChangesAsync();
    }

    private async Task DistributeSolutionsForTask(MaterialWorkModel task)
{
    // 1. Загружаем решения с необходимыми данными
    var solutions = await _context.Solutions
        .Where(s => s.TaskId == task.Id)
        .Include(s => s.Student)
        .Include(s => s.AttachedFiles)
        .ToListAsync();

    // 2. Фильтруем по дедлайну
    var onTimeSolutions = solutions
        .Where(s => s.SubmissionTime <= task.Deadline)
        .ToList();

    var lateSolutions = solutions
        .Where(s => s.SubmissionTime > task.Deadline)
        .ToList();

    // 3. Подготовка данных для распределения
    var checkAssignments = new List<SolutionForCheckModel>();
    var random = new Random();
    bool isTeacherCheckRequired = false;

    // 4. Словарь для учёта нагрузки студентов
    var reviewerLoad = new Dictionary<string, int>();
    foreach (var solution in onTimeSolutions)
    {
        reviewerLoad[solution.StudentId.ToString()] = 0;
    }

    // 5. Распределение P2P-проверок
    if (task.Check == Check.P2P && onTimeSolutions.Count >= 2 )
    {
        foreach (var solution in onTimeSolutions)
        {
            var neededChecks = task.SolutionsToCheckN;

            // Выбираем ревьюеров с минимальной текущей нагрузкой
            var availableReviewers = onTimeSolutions
                .Where(s => s.StudentId != solution.StudentId)
                .OrderBy(s => reviewerLoad[s.StudentId.ToString()])
                .ThenBy(_ => random.Next())
                .Take(neededChecks)
                .ToList();

            // Если не хватает ревьюеров, переключаемся на проверку преподавателем
            if (availableReviewers.Count < neededChecks)
            {
                lateSolutions =  solutions;
                isTeacherCheckRequired = true;
                break;
            }

            // Создаём проверки
            foreach (var reviewer in availableReviewers)
            {
                reviewerLoad[reviewer.StudentId.ToString()]++;

                var assessments = task.CriteriaAssignments
                    .Select(c => new AssessmentModel
                    {
                        Id = Guid.NewGuid(),
                        MaxScore = c.CountScore,
                        Remark = c.Conditions
                    })
                    .ToList();

                checkAssignments.Add(new SolutionForCheckModel
                {
                    Id = Guid.NewGuid(),
                    SolutionId = solution.Id,
                    AuthortId = reviewer.StudentId,
                    DueTime = DateTime.UtcNow.AddDays(7),
                    Assements = assessments,
                    Comment = string.Empty
                });
            }
        }
    }
    else
    {
        isTeacherCheckRequired = true;
        lateSolutions =  solutions;
    }

    // 6. Проверка преподавателем (для опоздавших или если P2P невозможно)
    if (isTeacherCheckRequired)
    {
        var solutionsToCheck = lateSolutions;
        
        foreach (var solution in solutionsToCheck)
        {
            var assessments = task.CriteriaAssignments
                .Select(c => new AssessmentModel
                {
                    Id = Guid.NewGuid(),
                    MaxScore = c.CountScore,
                    Remark = c.Conditions
                })
                .ToList();

            checkAssignments.Add(new SolutionForCheckModel
            {
                Id = Guid.NewGuid(),
                SolutionId = solution.Id,
                AuthortId = task.AuthorId,
                DueTime = DateTime.UtcNow.AddDays(7),
                Assements = assessments,
                Comment = string.Empty
            });
        }
    }

    // 7. Создаём пакет проверок
    if (checkAssignments.Any())
    {
        await _context.PackageChecks.AddAsync(new PackageCheckModel
        {
            Id = Guid.NewGuid(),
            SolutionForCheckTasks = checkAssignments,
            TaskId = task.Id,
            Deadline = DateTime.UtcNow.AddMinutes(15),
            IsTeacher = isTeacherCheckRequired,
            IsProcessed = false
        });

        task.SolutionsDistributed = true;
    }
}

}