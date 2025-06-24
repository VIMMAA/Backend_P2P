namespace Domain.Entities;

using Domain.Abstractions;
using Domain.Enums;
using System.Text.Json.Serialization;

public class CriteriaAssignment : Entity//раньше было название CriteriaAssignments
{
    public required string Title { get; set; }
    public required string Conditions { get; set; }
    public required int CountScore { get; set; }
    public Guid MaterialWorkModelId { get; set; }
    [JsonIgnore]
    public MaterialWorkModel? MaterialWorkModel { get; set; }
    public Guid? GradeModelId { get; set; }
    [JsonIgnore]
    public GradeModel? GradeModel { get; set; }

}