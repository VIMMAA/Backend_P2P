using System;
using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class SolutionModel
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
    }
}
