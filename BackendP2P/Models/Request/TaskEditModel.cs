namespace BackendP2P.Models.Request
{
    public class MaterialWorkEditModel
    {
        public required string Name { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
    }
}
