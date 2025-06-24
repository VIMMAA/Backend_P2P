namespace Domain.Entities;

using System.Text.Json.Serialization;
using Api.Models;
using Domain.Abstractions;
using Domain.Enums;
public class GradeModel : Entity//perhaps, you should check if Score exceeds sum of all criterias and give the result. Percentage or 5 grade system: 1/2/3/4/5
                                //as you wish, anyway
{
    public Guid? TeacherId { get; set; }
    [JsonIgnore]
    public UserModel? Teacher { get; set; }
    public int Score { get; set; }
    public Guid StudentId { get; set; }
    [JsonIgnore]
    public UserModel Student { get; set; }
    [JsonIgnore]
    public List<CriteriaAssignment> CriteriaAssignments { get; set; }
    [JsonIgnore]
    public SolutionCheck SolutionCheck { get; set; }
}