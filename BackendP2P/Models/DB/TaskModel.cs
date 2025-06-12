using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Domain.Abstractions;
using Domain.Entities;

namespace Api.Models;

public class TaskModel : Entity
{
    public required Guid AuthorId { get; set; }
    public required List<Guid> StudentGroup { get; set; }
    public List<Guid>? Solution { get; set; }
    [ForeignKey(nameof(Course))]
    public required Guid CourseId { get; set; }
    public required List<CommentModel>? Comments { get; set; }
    public required string Name { get; set; }
    [Required]
    public string Topic { get; set; } = default!;
    public required DateTime CreateTime { get; set; }
    public DateTime Deadline { get; set; }
    [JsonIgnore]
    public CourseModel? Course { get; set; }
}