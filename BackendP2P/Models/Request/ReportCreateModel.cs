namespace BackendP2P.Models.Request
{
    public class ReportCreateModel
    {
        public string Description { get; set; } = default!;


        public required Guid StudentId { get; set; }

        public required Guid SolutionId { get; set; }

    }
}
