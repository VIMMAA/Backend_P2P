using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class TaskGetModel
    {

        [Required]
        public string Name { get; set; } = default!;

        [Required]
        public string Topic { get; set; } = default!;

        //public List<Guid> StudentGroup { get; set; } = new();

        [Required]
        public DateTime Deadline { get; set; }
    }
}
