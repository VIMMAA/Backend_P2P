namespace BackendP2P.Models.Request
{
    public class CombinedMaterialWorkAndCriteriaModels
    {
        public TaskWorkCreateModel MaterialTaskWork { get; set; }
        public List<CriteriaAssignmentCreateModel> CriteriaAssignments { get; set; }
    }
}
