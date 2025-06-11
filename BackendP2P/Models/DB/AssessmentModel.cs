using System;
using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class AssessmentModel
    {
        public Guid? TeacherId { get; set; }

        [Range(0, 100)]
        public int Score { get; set; }

        [Required]
        public Guid StudentId { get; set; }
    }
}
