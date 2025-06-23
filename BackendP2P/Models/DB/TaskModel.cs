using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;

namespace Api.Models;

public class TaskModel : Entity
{
    public required Guid AuthorId { get; set; }
    [JsonIgnore]
    public UserModel? Author { get; set; }
    public List<Guid> Students { get; set; }
    [JsonIgnore]
    public List<SolutionModel>? Solutions { get; set; }
    [ForeignKey(nameof(Course))]
    public required Guid CourseId { get; set; }
    [JsonIgnore]
    public CourseModel? Course { get; set; }
    public List<CommentModel>? Comments { get; set; }
    public required string Name { get; set; }
    public string Topic { get; set; } = default!;
    public required DateTime CreateTime { get; set; }
    public DateTime Deadline { get; set; }
    public Check Check { get; set; }
    public Guid? MaterialReadId { get; set; }
    [JsonIgnore]
    public MaterialReadModel? MaterialReadModel { get; set; }
    public Guid? MaterialWorkId { get; set; }
    [JsonIgnore]
    public MaterialWorkModel? MaterialWorkModel { get; set; }
    [JsonIgnore]
    public CheckPackage CheckPackage { get; set; }
}