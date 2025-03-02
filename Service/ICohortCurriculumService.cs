using FOMSDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOMSService
{
    public interface ICohortCurriculumService
    {
        Task<IEnumerable<CohortCurriculumDTO>> GetCohortCurriculumAll();
        Task<CohortCurriculumDTO> GetCohortCurriculumByCohort(string cohort);
        Task Create(CohortCurriculumDTO cohortCurriculum);
        Task Update(CohortCurriculumDTO cohortCurriculum);
        Task Delete(string cohort);

    }
}
