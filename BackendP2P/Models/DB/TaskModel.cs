using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;

namespace Api.Models;

public class TaskModel : Entity
{
    //AUTHOR
    public required Guid AuthorId { get; set; }
    [JsonIgnore]
    public UserModel? Author { get; set; }
    //AUTHOR

    //GROUP
    public List<Guid> Students { get; set; }
    //GROUP

    //Solutions
    [JsonIgnore]
    public List<SolutionModel>? Solutions { get; set; }//2
    //Solutions

    //Course
    [ForeignKey(nameof(Course))]
    public required Guid CourseId { get; set; }
    [JsonIgnore]
    public CourseModel? Course { get; set; }
    //Course

    //Comments
    public List<CommentModel>? Comments { get; set; }
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

    public Check Check { get; set; }

    //Materials
    public Guid? MaterialReadId { get; set; }
    [JsonIgnore]
    public MaterialReadModel? MaterialReadModel { get; set; }
    public Guid? MaterialWorkId { get; set; }
    [JsonIgnore]
    public MaterialWorkModel? MaterialWorkModel { get; set; }
    //Materials

    //Grade
    [JsonIgnore]
    public List<GradeModel> Grades { get; set; }
    //Grade
}