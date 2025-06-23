namespace Domain.Entities;

using Api.Models;
using Domain.Abstractions;
using Domain.Enums;
using System.Text.Json.Serialization;

public class MaterialReadModel : Entity
{
    public string Content { get; set; }
    public Guid TaskId { get; set; }
    [JsonIgnore]
    public TaskModel Task { get; set; }
}