using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using FOMSDTO;
using System.Net.Http.Json;

namespace FOMSService
{
    public class OJTFeedbackService : IOJTFeedbackService
    {
        private readonly HttpClient _httpClient;
        public OJTFeedbackService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IEnumerable<OJTFeedbackDTO>> GetOJTFeedbackAll()
            => await _httpClient.GetFromJsonAsync<IEnumerable<OJTFeedbackDTO>>("OJTFeedback");
        public async Task<OJTFeedbackDTO> GetOJTFeedbackById(int id)
            => await _httpClient.GetFromJsonAsync<OJTFeedbackDTO>($"OJTFeedback/{id}");
        public async Task Create(OJTFeedbackDTO oJTFeedbackDTO)
            => await _httpClient.PostAsJsonAsync("OJTFeedback", oJTFeedbackDTO);
        public async Task Update(OJTFeedbackDTO oJTFeedbackDTO)
            => await _httpClient.PutAsJsonAsync($"OJTFeedback/{oJTFeedbackDTO.FeedbackId}", oJTFeedbackDTO);
        public async Task Delete(int id)
            => await _httpClient.DeleteAsync($"OJTFeedback/{id}");
    }
}
