using BusinessObject;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public interface ICohortCurriculumRepository
    {
        Task<IEnumerable<CohortCurriculum>> GetCohortCurriculumAll();
        Task<CohortCurriculum> GetCohortCurriculum(string cohort, int curriculumId);
        Task<List<CohortCurriculum>> GetCohortCurriculumByCohort(List<string> cohort);

        Task Create(CohortCurriculum cohortCurriculum);
        Task Update(CohortCurriculum cohortCurriculum);
        Task Delete(string cohort, int curriculumId);
        Task ImportCohortCurriculum(IEnumerable<CohortCurriculum> cohortCurriculum);
    }
}
