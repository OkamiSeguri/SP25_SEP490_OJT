using FOMSDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FOMSService
{
    public class CohortCurriculumService : ICohortCurriculumService
    {
        private readonly HttpClient _httpClient;
        public CohortCurriculumService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<CohortCurriculumDTO>> GetCohortCurriculumAll()
            => await _httpClient.GetFromJsonAsync<IEnumerable<CohortCurriculumDTO>>("CohortCurriculum");

        public async Task<CohortCurriculumDTO> GetCohortCurriculumByCohort(string cohort)
            => await _httpClient.GetFromJsonAsync<CohortCurriculumDTO>($"CohortCurriculum/{cohort}");

        public async Task Create(CohortCurriculumDTO cohortCurriculum)
            => await _httpClient.PostAsJsonAsync("CohortCurriculum", cohortCurriculum);

        public async Task Update(CohortCurriculumDTO cohortCurriculum)
            => await _httpClient.PutAsJsonAsync($"CohortCurriculum/{cohortCurriculum.Cohort}", cohortCurriculum);

        public async Task Delete(string cohort)
            => await _httpClient.DeleteAsync($"CohortCurriculum/{cohort}");

    }
}

