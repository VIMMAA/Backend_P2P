using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Api.Models;
public class ApplicationContext : DbContext
{
    public DbSet<UserModel> Users { get; set; }
    public DbSet<UserCorse> UsersCorses { get; set; }
    public DbSet<CourseModel> Courses {get;set;}
    public DbSet<TaskModel> Tasks { get; set; }
    public DbSet<SolutionModel> Solutions { get; set; }
    public DbSet<AssessmentModel> Assessments { get; set; }
    public DbSet<MaterialReadModel> MaterialReads { get; set; }
    public DbSet<MaterialWorkModel> MaterialWorks { get; set; }
    public DbSet<CriteriaAssignment> CriteriaAssignments { get; set; }
    public DbSet<GradeModel> Grades { get; set; }
    public DbSet<RemarkModel> Remarks { get; set; }
    public DbSet<SolutionCheck> SolutionChecks { get; set; }

    public bool TestConnection()
    {
        try
        {
            return Database.CanConnect();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка подключения к базе данных: {ex.Message}");
            return false;
        }
    }
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
}