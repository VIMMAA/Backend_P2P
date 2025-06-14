using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Domain.Abstractions;
using Domain.Entities;

namespace Api.Models;

public class TaskModel : Entity//2 fk - course&&user    //7 навигации - course+ &&user+&&list users+&&List comments+&&base class materials&&check list&&list solution+
{
    public required Guid AuthorId { get; set; }
    [JsonIgnore]
    public UserModel? Author { get; set; }//fk - 5 
    public Guid? StudentTaskId { get; set; }
    [JsonIgnore]
    public List<StudentTaskModel>? StudentTask { get; set; } = new();
    public List<SolutionModel>? Solution { get; set; }//2
    [ForeignKey(nameof(Course))]
    public required Guid CourseId { get; set; }
    public required List<CommentModel>? Comments { get; set; }//3
    public required string Name { get; set; }
    [Required]
    public string Topic { get; set; } = default!;
    public required DateTime CreateTime { get; set; }
    public DateTime Deadline { get; set; }
    [JsonIgnore]
    public CourseModel? Course { get; set; }//fk - 4
    [JsonIgnore]
    public List<MaterialReadModel>? MaterialReadModel { get; set; }//возможно и 1 материал максимум - потом посмотрю
    [JsonIgnore]
    public MaterialWorkModel? MaterialWorkModel { get; set; }
}