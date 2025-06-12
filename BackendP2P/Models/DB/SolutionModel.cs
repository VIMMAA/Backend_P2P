using System;
using System.ComponentModel.DataAnnotations;
using Domain.Abstractions;

namespace Api.Models
{
    public class SolutionModel : Entity
    {
        [Required]
        public DateTime SubmissionTime { get; set; }

        [Required]
        public Guid StudentId { get; set; }

        [Required]
        public AssessmentModel Assessment { get; set; }

        [Required]
        public string Content { get; set; } = default!;

        [Required]
        public string ValidationPackage { get; set; } = default!;

        [Required]
        public string AttachmentPath { get; set; } = default!;
        [Required]
        public Guid taskId { get; set; }
    }
}
