namespace Domain.Entities;

using Api.Models;
using Domain.Abstractions;
using Domain.Enums;
public class MaterialReadModel : Entity
{
    public string Content { get; set; }
    public Guid TaskId { get; set; }
    public TaskModel Task { get; set; }
}