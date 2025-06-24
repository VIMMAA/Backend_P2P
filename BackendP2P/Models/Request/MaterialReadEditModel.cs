namespace BackendP2P.Models.Request
{
    public class MaterialReadEditModel
    {
        public required string Name { get; set; }
        public string Topic { get; set; } = default!;
        public string Content { get; set; }
    }
}
