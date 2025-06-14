namespace Domain.Entities;

using Domain.Abstractions;
using Domain.Enums;
public class CriteriaAssignments : Entity
{
    public required string Title { get; set; }
    public required string Conditions { get; set; }
    public required string CountScore { get; set; }
    public required string Level { get; set; }
}