using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Domain.Abstractions;
using Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;  
using System;

namespace Api.Models
{
    public class GradeShortModel 
    {

        public Guid GradeId { get; set; }

        public int Score { get; set; }

        public string TaskName { get; set; }
        
        public Guid SolutionId { get; set; }

    }
}