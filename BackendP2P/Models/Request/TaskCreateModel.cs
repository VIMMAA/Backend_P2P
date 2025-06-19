using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{
    public class TaskCreateModel
    {
        public required string Name { get; set; }
        public List<Guid> Students { get; set; }
        public string Topic { get; set; } = default!;
        public DateTime Deadline { get; set; }
    }
}
