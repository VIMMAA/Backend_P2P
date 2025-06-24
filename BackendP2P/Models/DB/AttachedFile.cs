using Domain.Abstractions;
using System.Text.Json.Serialization;

namespace Api.Models
{
    public class AttachedFileModel : Entity
    {
        public string Name { get; set; } = default!;
        public string Data { get; set; } = default!;
        public Guid SolutionId { get; set; }
        [JsonIgnore]
        public SolutionModel Solution { get; set; } = default!;
    }
}
