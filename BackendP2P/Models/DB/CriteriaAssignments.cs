namespace Domain.Entities;

using Domain.Abstractions;
using Domain.Enums;
public class CriteriaAssignment : Entity//раньше было название CriteriaAssignments
{
    public required string Title { get; set; }
    public required string Conditions { get; set; }
    public required string CountScore { get; set; }
    public required string Level { get; set; }
    public Guid MaterialWorkModelId { get; set; }
    public MaterialWorkModel? MaterialWorkModel { get; set; }
}