using Api.Models;
using Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendP2P.Models.Request
{
    public class TaskCreatedModel
    {
        public required Guid Id { get; set; }
        public required Guid AuthorId { get; set; }
        public List<Guid> Students { get; set; }
        public List<SolutionModel>? Solutions { get; set; }
        public required Guid CourseId { get; set; }
        public required List<CommentModel>? Comments { get; set; }
        public required string Name { get; set; }
        public string Topic { get; set; } = default!;
        public required DateTime CreateTime { get; set; }
        public DateTime Deadline { get; set; }
        public Guid? MaterialId { get; set; }
        public List<GradeModel> Grades { get; set; }
        public Guid? MaterialWorkId { get; set; }
        public Guid? MaterialReadId { get; set; }
    }
}
