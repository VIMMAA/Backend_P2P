using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class DeadlineProcessingService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<DeadlineProcessingService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(0.1); // Проверяем каждый час

    public DeadlineProcessingService(
        IServiceProvider services, 
        ILogger<DeadlineProcessingService> logger)
    {
        _services = services;
        _logger = logger;
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
                _logger.LogError(ex, "Ошибка при обработке истекших дедлайнов");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task ProcessExpiredDeadlines(ApplicationContext context)
    {
        var now = DateTime.UtcNow;
        var expiredPackages = await context.PackageChecks
            .Where(p => !p.IsProcessed && p.Deadline <= now)
            .Include(p => p.SolutionForCheckTasks)
            .ThenInclude(s => s.Solution)
            .ToListAsync();

        foreach (var package in expiredPackages)
        {
            try
            {
                // 1. Распределяем оставшиеся решения на проверку преподавателям
                await DistributeRemainingSolutions(context, package);

                // 2. Рассчитываем итоговые оценки
                await CalculateFinalGrades(context, package);

                // 3. Отправляем уведомления
                await SendNotifications(context, package);

                // Помечаем как обработанное
                package.IsProcessed = true;
                await context.SaveChangesAsync();
                
                _logger.LogInformation($"Обработан PackageCheck {package.Id} с дедлайном {package.Deadline}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при обработке PackageCheck {package.Id}");
            }
        }
    }

    private async Task DistributeRemainingSolutions(
        ApplicationContext context, 
        PackageCheckModel package)
    {
        // Находим решения без проверок
        var solutionsWithoutChecks = package.SolutionForCheckTasks
            .Where(s => !s.IsChecked)
            .ToList();

        foreach (var solution in solutionsWithoutChecks)
        {
            // Назначаем проверку преподавателю курса
            var teacher = await context.UsersCorses
                .Where(uc => uc.CourseId == solution.Solution.Task.CourseId && 
                            uc.Role == Role.Teacher)
                .Select(uc => uc.User)
                .FirstOrDefaultAsync();

            if (teacher != null)
            {
                var checkTask = new SolutionForCheckModel
                {
                    SolutionId = solution.SolutionId,
                    AuthortId = teacher.Id,
                    DueTime = DateTime.UtcNow.AddDays(3), // Даем 3 дня на проверку
                    Comment = "Автоматически назначено после дедлайна"
                };

                context.SolutionForChecks.Add(checkTask);
            }
        }

        await context.SaveChangesAsync();
    }

    private async Task CalculateFinalGrades(
        ApplicationContext context, 
        PackageCheckModel package)
    {
        foreach (var solution in package.SolutionForCheckTasks)
        {
            if (solution.IsChecked && solution.Assements.Any())
            {
                var totalScore = solution.Assements.Sum(a => a.Score);
                var maxScore = solution.Assements.Sum(a => a.MaxScore);
                
                // Сохраняем итоговую оценку
                var grade = new GradeModel
                {
                    SolutionId = solution.SolutionId,
                    Score = totalScore,
                    MaxScore = maxScore,
                    GradedAt = DateTime.UtcNow
                };

                context.Grades.Add(grade);
            }
        }

        await context.SaveChangesAsync();
    }

    private async Task SendNotifications(
        ApplicationContext context, 
        PackageCheckModel package)
    {
        // Логика отправки уведомлений...
        // Можно интегрировать с EmailService или системой веб-уведомлений
    }
}