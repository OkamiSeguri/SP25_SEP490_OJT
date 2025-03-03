using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FOMSDTO;
using System.Net.Http.Json;

namespace FOMSService
{
    public class OJTResultService : IOJTResultService
    {
        private readonly HttpClient _httpClient;
        public OJTResultService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IEnumerable<OJTResultDTO>> GetOJTResultAll()
            => await _httpClient.GetFromJsonAsync<IEnumerable<OJTResultDTO>>("OJTResult");
        public async Task<OJTResultDTO> GetOJTResultById(int id)
            => await _httpClient.GetFromJsonAsync<OJTResultDTO>($"OJTResult/{id}");
        public async Task Create(OJTResultDTO ojtResultDTO)
            => await _httpClient.PostAsJsonAsync("OJTResult", ojtResultDTO);
        public async Task Update(OJTResultDTO ojtResultDTO)
            => await _httpClient.PutAsJsonAsync($"OJTResult/{ojtResultDTO.OJTId}", ojtResultDTO);
        public async Task Delete(int id)
            => await _httpClient.DeleteAsync($"OJTResult/{id}");
    }
}
