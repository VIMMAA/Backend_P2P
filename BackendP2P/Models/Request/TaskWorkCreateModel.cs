using Domain.Enums;

namespace BackendP2P.Models.Request
{
    public class TaskWorkCreateModel
    {
        public string Name { get; set; }
        public string Topic { get; set; }
        public DateTime Deadline { get; set; }
        public Check Check { get; set; }
        public string Instructions { get; set; }
        public double? Penalty { get; set; }
        public int? SolutionsToCheckN { get; set; }
        public bool isP2P { get; set; }
    }
}
