using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Domain.Abstractions;
using Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;  

namespace Api.Models
{
    public class PackageShort 
    {

        public List<SolutionForCheckModel> SolutionForCheckTasks { get; set; }

        public Guid TaskId { get; set; }

        [Column(TypeName = "timestamp with time zone")]
        public DateTime Deadline { get; set; }


    }
}