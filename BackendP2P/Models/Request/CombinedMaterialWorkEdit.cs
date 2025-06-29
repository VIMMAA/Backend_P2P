namespace BackendP2P.Models.Request
{
    public class CombinedMaterialWorkEdit
    {
        public MaterialWorkEditModel MaterialWorkEdit { get; set; }
        public List<CriteriaAssignmentCreateModel> CriteriaAssignments { get; set; }
        public List<AttachedFileDto>? Files { get; set; }
    }
}
