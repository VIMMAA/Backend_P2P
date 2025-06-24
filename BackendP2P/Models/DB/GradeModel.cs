namespace Domain.Entities;

using System.Text.Json.Serialization;
using Api.Models;
using Domain.Abstractions;
using Domain.Enums;
public class GradeModel : Entity
{
    public Guid? TeacherId { get; set; }
    [JsonIgnore]
    public UserModel? Teacher { get; set; }
    public int Score { get; set; }
    public Guid StudentId { get; set; }
    public string Remark { get; set; }
    [JsonIgnore]
    public UserModel Student { get; set; }
    [JsonIgnore]
    public List<CriteriaAssignment> CriteriaAssignments { get; set; }
    [JsonIgnore]
    public SolutionCheck SolutionCheck { get; set; }
}