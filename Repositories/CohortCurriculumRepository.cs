using BusinessObject;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class CohortCurriculumRepository : ICohortCurriculumRepository
    {
        public async Task<IEnumerable<CohortCurriculum>> GetCohortCurriculumAll()
        {
            return await CohortCurriculumDAO.Instance.GetCohortCurriculumAll();
        }

        public async Task<CohortCurriculum> GetCohortCurriculum(int cohortcurriculumId)
        {
            return await CohortCurriculumDAO.Instance.GetCohortCurriculum(cohortcurriculumId);
        }

        public async Task Create(CohortCurriculum cohortCurriculum)
        {
            await CohortCurriculumDAO.Instance.Create(cohortCurriculum);
        }
        public async Task Update(CohortCurriculum cohortCurriculum)
        {
            await CohortCurriculumDAO.Instance.Update(cohortCurriculum);
        }
        public async Task Delete(int cohortcurriculumId)
        {
            await CohortCurriculumDAO.Instance.Delete(cohortcurriculumId);
        }

    }
}
