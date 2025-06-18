using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Domain.Abstractions;
using Domain.Entities;

namespace Api.Models
{
    public class SolutionModel : Entity
    {
        public DateTime SubmissionTime { get; set; }
        public Guid StudentId { get; set; }
        [JsonIgnore]
        public UserModel? Student { get; set; }
        //public Guid AssessmentId { get; set; } //я пока не добавляю gradeModel - в таске уже есть грейд
        public string Content { get; set; } = default!;
        public string AttachmentPath { get; set; } = default!;
        public Guid TaskId { get; set; }
        [JsonIgnore]
        public TaskModel? Task { get; set; }//3
    }
}
