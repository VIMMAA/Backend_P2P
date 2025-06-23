using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Domain.Abstractions;
using Domain.Entities;

namespace Api.Models;

public class CommentModel : Entity
{
    public required string Text { get; set; }

    public required Guid AuthorId { get; set; }
    [JsonIgnore]
    public UserModel Author { get; set; }
    public required DateTime CreateTime { get; set; }

}