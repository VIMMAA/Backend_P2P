using Api.Models;
using Domain.Entities;

namespace BackendP2P.Models.Request
{
    public class AssessmentList
    {
        public List<SolutionForCheckModel> SolutionForCheckModels { get; set; } = default!;

        public GradeModel? Grade { get; set; }

        public SolutionForCheckModel? TeacherAssessment {get; set;}
    }
}
