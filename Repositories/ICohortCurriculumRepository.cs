using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface ICohortCurriculumRepository
    {
        Task<IEnumerable<CohortCurriculum>> GetCohortCurriculumAll();
        Task<CohortCurriculum> GetCohortCurriculumByCohort(string cohort);
        Task Create(CohortCurriculum cohortCurriculum);
        Task Update(CohortCurriculum cohortCurriculum);
        Task Delete(string cohort);
    }
}

