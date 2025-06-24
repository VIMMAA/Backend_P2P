using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Api.Models;
using Domain.Abstractions;
using Domain.Entities;

public class SolutionCheck : Entity
{
    [Required]
    public DateTime SubmissionTime { get; set; }
    [Required]
    public Guid AuthorId { get; set; }
    [JsonIgnore]
    public UserModel Author { get; set; }
    public Guid? GradeId { get; set; }
    [JsonIgnore]
    public GradeModel? Grade { get; set; }
    [Required]
    public string Content { get; set; }
    [Required]
    public Guid SolutionId { get; set; }
    [JsonIgnore]
    public SolutionModel Solution { get; set; }
    public Guid? RemarkId { get; set; }
    [JsonIgnore]
    public RemarkModel? Remark{ get; set; }
    //public Guid CheckPackageId { get; set; }
    //[JsonIgnore]
    //public CheckPackage CheckPackage { get; set; }
    [Required]
    public string AttachmentPath { get; set; } = default!;
}