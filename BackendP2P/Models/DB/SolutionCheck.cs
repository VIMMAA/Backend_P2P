using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Api.Models;
using Domain.Abstractions;
using Domain.Entities;

public class SolutionCheck : Entity//пакет проверки потом добавить
{
    [Required]
    public DateTime SubmissionTime { get; set; }
    [Required]
    public Guid AuthorId { get; set; }
    public UserModel Author { get; set; }//1
    public GradeModel? GradeModel { get; set; }
    [Required]
    public string Content { get; set; }
    [Required]
    public Guid SolutionId { get; set; }
    public SolutionModel Solution { get; set; }
    [Required]
    public string AttachmentPath { get; set; } = default!;
    public RemarkModel? Remark{ get; set; }
}