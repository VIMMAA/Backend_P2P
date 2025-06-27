using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Domain.Abstractions;
using Domain.Entities;

namespace Api.Models
{
    public class AssessmentModel : Entity//2 fk         навигации - 2
    {

        public int? Score { get; set; }

        public int MaxScore { get; set; }
        public string Remark { get; set; }
        
    }
}
