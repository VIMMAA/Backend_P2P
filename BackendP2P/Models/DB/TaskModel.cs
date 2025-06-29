using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;

namespace Api.Models;

public abstract class TaskModel : Entity
{
    public required Guid AuthorId { get; set; }
    [ForeignKey(nameof(Course))]
    public required Guid CourseId { get; set; }
    public required string Name { get; set; }
    public string Description { get; set; }
    public required DateTime CreateTime { get; set; }
    [JsonIgnore]
    public UserModel? Author { get; set; }
    [JsonIgnore]
    public CourseModel? Course { get; set; }

    public List<CommentModel>? Comments { get; set; }
    public List<AttachedFileModel>? AttachedFiles { get; set; }
}