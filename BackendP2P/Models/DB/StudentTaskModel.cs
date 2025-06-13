using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Domain.Abstractions;
using Domain.Entities;

namespace Api.Models;

public class StudentTaskModel : Entity
{
    public required Guid StudentId { get; set; }
    [JsonIgnore]
    public UserModel Student { get; set; }
    public required Guid TaskId { get; set; } 
    [JsonIgnore]
    public TaskModel Task { get; set; } 
}