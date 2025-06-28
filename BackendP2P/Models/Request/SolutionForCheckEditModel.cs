using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Domain.Abstractions;
using Domain.Entities;

namespace Api.Models
{
    public class SolutionForCheckEditModel
    {


        [Required]
        public List<AssessmentModel> Assements { get; set; }

        public string Comment { get; set; }
        
    }
}