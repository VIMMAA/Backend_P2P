namespace BackendP2P.Models.Request
{
    public class TaskEditModel
    {
        public required string Name { get; set; }
        public List<Guid> Students { get; set; }
        public string Topic { get; set; } = default!;
        public DateTime Deadline { get; set; }
    }
}
