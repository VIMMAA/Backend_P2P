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
    public double Penalty { get; set; } = 0.3;
    public int SolutionsToCheckN { get; set; } = 2;

    public bool SolutionsDistributed { get; set; } = false;

    public List<SolutionModel>? Solutions { get; set; }
}