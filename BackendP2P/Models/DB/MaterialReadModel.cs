namespace Domain.Entities;

using Api.Models;
using Domain.Abstractions;
using Domain.Enums;
using System.Text.Json.Serialization;

public class MaterialReadModel : TaskModel
{
    public string Content { get; set; }
}