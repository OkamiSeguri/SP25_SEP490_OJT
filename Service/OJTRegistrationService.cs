using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FOMSDTO;
using System.Net.Http.Json;

namespace FOMSService
{
    public class OJTRegistrationService : IOJTRegistrationService
    {
        private readonly HttpClient _httpClient;
        public OJTRegistrationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IEnumerable<OJTRegistrationDTO>> GetOJTRegistrationAll()
            => await _httpClient.GetFromJsonAsync<IEnumerable<OJTRegistrationDTO>>("OJTRegistration");
        public async Task<OJTRegistrationDTO> GetOJTRegistrationById(int id)
            => await _httpClient.GetFromJsonAsync<OJTRegistrationDTO>($"OJTRegistration/{id}");
        public async Task Create(OJTRegistrationDTO oJTRegistrationDTO)
            => await _httpClient.PostAsJsonAsync("OJTRegistration", oJTRegistrationDTO);
        public async Task Update(OJTRegistrationDTO oJTRegistrationDTO)
            => await _httpClient.PutAsJsonAsync($"OJTRegistration/{oJTRegistrationDTO.ProgramId}", oJTRegistrationDTO);
        public async Task Delete(int id)
            => await _httpClient.DeleteAsync($"OJTRegistration/{id}");
    }
}
