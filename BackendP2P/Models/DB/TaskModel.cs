using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Domain.Abstractions;
using Domain.Entities;

namespace Api.Models;

public class TaskModel : Entity
{
    //AUTHOR
    public required Guid AuthorId { get; set; }
    public UserModel? Author { get; set; }
    //AUTHOR

    //GROUP
    public List<UserModel> Students { get; set; }
    //GROUP

    //Solutions
    public List<SolutionModel>? Solutions { get; set; }//2
    //Solutions

    //Course
    [ForeignKey(nameof(Course))]
    public required Guid CourseId { get; set; }
    public CourseModel? Course { get; set; }
    //Course

    //Comments
    public required List<CommentModel>? Comments { get; set; }
    //Comments

    //Name
    public required string Name { get; set; }
    //Name

    //Topic
    public string Topic { get; set; } = default!;
    //Topic
    
    //CreateTime
    public required DateTime CreateTime { get; set; }
    //CreateTime

    //Deadline
    public DateTime Deadline { get; set; }
    //Deadline

    //Materials
    public List<MaterialReadModel>? MaterialReadModel { get; set; }
    public MaterialWorkModel? MaterialWorkModel { get; set; }
    //Materials

    //Grade
    public Guid GradeId { get; set; }
    public GradeModel Grade { get; set; }
    //Grade
}