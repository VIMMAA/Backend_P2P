namespace Domain.Entities;

using Api.Models;
using Domain.Abstractions;
using Domain.Enums;
public class MaterialWorkModel : Entity
{
    public string Score { get; set; }
    public DateTime Deadline { get; set; }
    public string Instructions { get; set; }
    public List<CriteriaAssignment>? CriteriaAssignments { get; set; }
    public Guid TaskId { get; set; }
    public TaskModel Task { get; set; }
}