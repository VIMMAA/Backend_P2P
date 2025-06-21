using Api.Models;
using Domain.Entities;
using System.Text.Json.Serialization;

namespace BackendP2P.Models.Request
{
    public class SolutionCreateModel
    {
        public string Content { get; set; } = default!;
        public string AttachmentPath { get; set; } = default!;
    }
}