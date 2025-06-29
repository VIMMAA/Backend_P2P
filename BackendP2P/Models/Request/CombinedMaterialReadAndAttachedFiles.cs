using Api.Models;

namespace BackendP2P.Models.Request
{
    public class CombinedMaterialReadAndAttachedFiles
    {
        public MaterialReadCreate MaterialReadCreate { get; set; }
        public List<AttachedFileDto>? Files { get; set; }
    }
}
