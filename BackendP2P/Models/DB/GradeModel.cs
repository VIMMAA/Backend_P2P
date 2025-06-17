namespace Domain.Entities;

using System.Text.Json.Serialization;
using Api.Models;
using Domain.Abstractions;
using Domain.Enums;
public class GradeModel : Entity
{
    public Guid? TeacherId { get; set; }
    public UserModel? Teacher { get; set; }
    public int Score { get; set; }
    public Guid StudentId { get; set; }
    public UserModel Student { get; set; }
    public Guid SolutionCheckId { get; set; }
    public SolutionCheck SolutionCheck { get; set; }
}