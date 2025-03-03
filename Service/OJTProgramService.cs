using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FOMSDTO;
using System.Net.Http.Json;

namespace FOMSService
{
    public class OJTProgramService : IOJTProgramService
    {
        private readonly HttpClient _httpClient;
        public OJTProgramService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IEnumerable<OJTProgramDTO>> GetOJTProgramAll()
            => await _httpClient.GetFromJsonAsync<IEnumerable<OJTProgramDTO>>("OJTProgram");
        public async Task<OJTProgramDTO> GetOJTProgramById(int id)
            => await _httpClient.GetFromJsonAsync<OJTProgramDTO>($"OJTProgram/{id}");
        public async Task Create(OJTProgramDTO oJTProgramDTO)
            => await _httpClient.PostAsJsonAsync("OJTProgram", oJTProgramDTO);
        public async Task Update(OJTProgramDTO oJTProgramDTO)
            => await _httpClient.PutAsJsonAsync($"OJTProgram/{oJTProgramDTO.ProgramId}", oJTProgramDTO);
        public async Task Delete(int id)
            => await _httpClient.DeleteAsync($"OJTProgram/{id}");
    }
}
