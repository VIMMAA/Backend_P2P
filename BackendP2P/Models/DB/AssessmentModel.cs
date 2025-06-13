using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Domain.Abstractions;
using Domain.Entities;

namespace Api.Models
{
    public class AssessmentModel : Entity//2 fk         навигации - 2
    {
        public Guid? TeacherId { get; set; }

        [Range(0, 100)]
        public int Score { get; set; }

        [Required]
        public Guid StudentId { get; set; }

        [JsonIgnore]
        public UserModel? Student { get; set; }

        [JsonIgnore]
        public UserModel? Teacher { get; set; }

    }
}
