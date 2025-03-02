using FOMSDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FOMSService
{
    public class CurriculumService : ICurriculumService
    {
        private readonly HttpClient _httpClient;
        public CurriculumService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<CurriculumDTO>> GetCurriculumAll()
            => await _httpClient.GetFromJsonAsync<IEnumerable<CurriculumDTO>>("Curriculum");

        public async Task<CurriculumDTO> GetCurriculumById(int id)
            => await _httpClient.GetFromJsonAsync<CurriculumDTO>($"Curriculum/{id}");

        public async Task Create(CurriculumDTO curriculumDTO)
            => await _httpClient.PostAsJsonAsync("Curriculum", curriculumDTO);

        public async Task Update(CurriculumDTO curriculumDTO)
            => await _httpClient.PutAsJsonAsync($"Curriculum/{curriculumDTO.CurriculumId}", curriculumDTO);

        public async Task Delete(int id)
            => await _httpClient.DeleteAsync($"Curriculum/{id}");

    }
}
