using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Domain.Abstractions;
using Domain.Entities;

namespace Api.Models
{
    public class PackageCheckModel : Entity
    {

        public List<SolutionForCheckModel> SolutionForCheckTasks { get; set; }

        public Guid TaskId { get; set; }
        public DateTime Deadline { get; set; }

        public bool IsProcessed { get; set; } = false;

        public bool IsTeacher { get; set; }



    }
}