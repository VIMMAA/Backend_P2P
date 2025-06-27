namespace BackendP2P.Models.Request
{
    public class ReportCreateModel
    {
        public string Description { get; set; } = default!;

        public string Theme { get; set; }

        public Guid StudentId { get; set; }

    }
}
