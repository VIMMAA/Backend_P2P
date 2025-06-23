namespace Domain.Entities;

using System.Text.Json.Serialization;
using Api.Models;
using Domain.Abstractions;
using Domain.Enums;
public class RemarkModel : Entity
{
    public string Content { get; set; }
    public DateTime Date { get; set; }
    public Guid AuthorId { get; set; }
    [JsonIgnore]
    public UserModel Author { get; set; }
    [JsonIgnore]
    public SolutionCheck SolutionCheck { get; set; }
}