
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Api.Models;
using Domain.Enums;
public class DeadlineProcessingService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<DeadlineProcessingService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(10);

    private readonly ApplicationContext _context;


    public DeadlineProcessingService(ApplicationContext context, IServiceProvider services, ILogger<DeadlineProcessingService> logger)
    {
        _services = services;
        _logger = logger;
        _context = context;

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                await ProcessExpiredDeadlines(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing deadlines");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task ProcessExpiredDeadlines(ApplicationContext context)
    {
        var now = DateTime.UtcNow;
        var expiredPackages = await context.PackageChecks
            .Where(p => !p.IsProcessed && p.Deadline <= now && p.IsTeacher == false)
            .Include(p => p.SolutionForCheckTasks)
            .ThenInclude(s => s.Assements)
            .Include(p => p.SolutionForCheckTasks)
            .ThenInclude(s => s.Solution)
            .ThenInclude(s => s.Task)
            .ToListAsync();

        foreach (var package in expiredPackages)
        {
            try
            {
                await CalculateAndSaveFinalGrades(context, package);
                package.IsProcessed = true;
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing package {package.Id}");
            }
        }
    }

    private async Task CalculateAndSaveFinalGrades(ApplicationContext context, PackageCheckModel package)
    {
        var solutionsGroups = package.SolutionForCheckTasks
        .GroupBy(s => s.SolutionId);

        foreach (var group in solutionsGroups)
        {
            var sum = 0.0;
            var solution = group.First().Solution; 
            if (solution == null) continue;


            Console.WriteLine($"Обрабатываем решение: {group.Key}");

            var firstCheckWithAssessments = group.FirstOrDefault(c => c.Assements?.Any() == true);
            if (firstCheckWithAssessments == null) continue;

            int assessmentsCount = firstCheckWithAssessments.Assements.Count;

            for (int i = 0; i < assessmentsCount; i++)
            {
                var positionScores = group
                    .Where(c => c.Assements != null && c.Assements.Count > i)
                    .Select(c => c.Assements[i])
                    .Where(a => a.Score.HasValue)
                    .ToList();

                if (positionScores.Any())
                {
                    double averageScore = positionScores.Average(a => a.Score.Value);
                    sum += averageScore;
                    Console.WriteLine($"Среднее для оценки #{i + 1}: {averageScore:F2}");
                }
            }
            var fin = (int)Math.Round(sum, MidpointRounding.AwayFromZero);

            bool isAllChecked = !_context.SolutionForChecks
                .Any(s => s.AuthortId == solution.StudentId && !s.IsChecked);

            var task = _context.MaterialWorks.FirstOrDefault(t => t.Id == solution.TaskId);

            GradeModel grade = new GradeModel
            {
                Id = Guid.NewGuid(),
                Score = isAllChecked ? fin : (int)(fin * task.Penalty),
                StudentId = solution.StudentId,
                TaskId = solution.TaskId,
                Remark = "",
            };

            _context.Grades.Add(grade);
        }

        await context.SaveChangesAsync();
    }
}
