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
        public required DateTime CreateTime { get; set; }
        public DateTime Deadline { get; set; }
        public Guid MaterialReadId { get; set; }
        public Guid MaterialWorkId { get; set; }
    }
}
