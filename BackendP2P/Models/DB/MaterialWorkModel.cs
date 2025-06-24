namespace Domain.Entities;

using Api.Models;
using Domain.Abstractions;
using Domain.Enums;
using System.Text.Json.Serialization;

public class MaterialWorkModel : TaskModel
{
    public int Score { get; set; }
    public DateTime Deadline { get; set; }
    public Check Check { get; set; }
    public string Instructions { get; set; }
    public List<CriteriaAssignment>? CriteriaAssignments { get; set; }
    public double? Penalty { get; set; } = 0.3;
    //[JsonIgnore]
    //public CheckPackage CheckPackage { get; set; }
    public List<SolutionModel>? Solutions { get; set; }
}