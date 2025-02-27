using FOMSDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FOMSService
{
    public class EnterpriseService : IEnterpriseService
    {
    
    private readonly HttpClient _httpClient;
    public EnterpriseService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<EnterpriseDTO>> GetEnterpriseAll()
        => await _httpClient.GetFromJsonAsync<IEnumerable<EnterpriseDTO>>("Enterprise");

    public async Task<EnterpriseDTO> GetEnterpriseById(int id)
        => await _httpClient.GetFromJsonAsync<EnterpriseDTO>($"Enterprise/{id}");

    public async Task Create(EnterpriseDTO enterpriseDTO)
        => await _httpClient.PostAsJsonAsync("Enterprise", enterpriseDTO);

    public async Task Update(EnterpriseDTO enterpriseDTO)
        => await _httpClient.PutAsJsonAsync($"Enterprise/{enterpriseDTO.EnterpriseId}", enterpriseDTO);
    public async Task Delete(int id)
        => await _httpClient.DeleteAsync($"Enterprise/{id}");

}
}
