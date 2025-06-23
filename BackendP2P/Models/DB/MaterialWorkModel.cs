namespace Domain.Entities;

using Api.Models;
using Domain.Abstractions;
using Domain.Enums;
using System.Text.Json.Serialization;

public class MaterialWorkModel : Entity
{
    public int Score { get; set; }
    public DateTime Deadline { get; set; }
    public string Instructions { get; set; }
    [JsonIgnore]
    public List<CriteriaAssignment>? CriteriaAssignments { get; set; }
    public Guid TaskId { get; set; }
    [JsonIgnore]
    public TaskModel Task { get; set; }
}