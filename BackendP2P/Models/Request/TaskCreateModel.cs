using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class TaskCreateModel
    {
        [Required]
        public string Title { get; set; } = default!;

        [Required]
        public string Topic { get; set; } = default!;

        [Required]
        public List<Guid> StudentGroup { get; set; } = new();

        [Required]
        public DateTime DueDate { get; set; }
    }
}
