using BusinessObject;
using DataAccess;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public class CohortCurriculumRepository : ICohortCurriculumRepository
    {
        public async Task<IEnumerable<CohortCurriculum>> GetCohortCurriculumAll()
        {
            return await CohortCurriculumDAO.Instance.GetCohortCurriculumAll();
        }

        public async Task<CohortCurriculum> GetCohortCurriculum(string cohort, int curriculumId)
        {
            return await CohortCurriculumDAO.Instance.GetCohortCurriculum(cohort, curriculumId);
        }       
        public async Task<List<CohortCurriculum>> GetCohortCurriculumByCohort(List<string> cohort)
        {
            return await CohortCurriculumDAO.Instance.GetCohortCurriculumByCohort(cohort);

        }

        public async Task Create(CohortCurriculum cohortCurriculum)
        {
            await CohortCurriculumDAO.Instance.Create(cohortCurriculum);
        }

        public async Task Update(CohortCurriculum cohortCurriculum)
        {
            await CohortCurriculumDAO.Instance.Update(cohortCurriculum);
        }

        public async Task Delete(string cohort, int curriculumId)
        {
            await CohortCurriculumDAO.Instance.Delete(cohort, curriculumId);
        }

        public async Task ImportCohortCurriculum(IEnumerable<CohortCurriculum> cohortCurriculum)
        {
            await CohortCurriculumDAO.Instance.ImportCohortCurriculumAsync(cohortCurriculum);
        }
    }
}
