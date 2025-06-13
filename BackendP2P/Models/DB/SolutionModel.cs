using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Domain.Abstractions;
using Domain.Entities;

namespace Api.Models
{
    public class SolutionModel : Entity//fk - 3     навигации - 3
    {
        [Required]
        public DateTime SubmissionTime { get; set; }

        [Required]
        public Guid StudentId { get; set; }
        [JsonIgnore]
        public UserModel? Student { get; set; }//1

        [Required]
        public Guid AssessmentId { get; set; }
        [JsonIgnore]
        public AssessmentModel Assessment { get; set; }//2

        [Required]
        public string Content { get; set; } = default!;

        [Required]
        public string AttachmentPath { get; set; } = default!;
        [Required]
        public Guid taskId { get; set; }
        [JsonIgnore]
        public TaskModel? task { get; set; }//3
    }
}
