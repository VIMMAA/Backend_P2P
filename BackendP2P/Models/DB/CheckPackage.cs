using System.Text.Json.Serialization;
using Domain.Abstractions;
using Domain.Entities;

namespace Api.Models
{
    public class CheckPackage : Entity
    {
        public Guid UserId { get; set; }
        [JsonIgnore]
        public UserModel User { get; set; }
        public DateTime Deadline { get; set; }
        public string Instructions { get; set; }
        public Guid TaskId { get; set; }
        [JsonIgnore]
        public TaskModel Task { get; set; }
        [JsonIgnore]
        public List<SolutionCheck> SolutionChecks { get; set; }
    }
}
