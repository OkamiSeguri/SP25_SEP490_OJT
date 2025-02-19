using FOMSDTO;
using FOMSService;
using System.Net.Http.Json;

public class Userservice : IUserservice
{
    private readonly HttpClient _httpClient;
    public Userservice(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<UserDTO>> GetUserAll()
        => await _httpClient.GetFromJsonAsync<IEnumerable<UserDTO>>("User");

    public async Task<UserDTO> GetUserById(int id)
        => await _httpClient.GetFromJsonAsync<UserDTO>($"User/{id}");

    public async Task Create(UserDTO userDTO)
        => await _httpClient.PostAsJsonAsync("User", userDTO);

    public async Task Update(UserDTO userDTO)
        => await _httpClient.PutAsJsonAsync($"User/{userDTO.UserId}", userDTO);

    public async Task Delete(int id)
        => await _httpClient.DeleteAsync($"User/{id}");

    public async Task<UserDTO?> Login(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("User/Login", new { Email = email, Password = password });

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<UserDTO>();
        }
        return null;
    }
    public async Task<UserDTO> Register(RegisterDTO registerDTO)
    {
        var response = await _httpClient.PostAsJsonAsync("User/Register", registerDTO);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<UserDTO>();
        }
        return null;
    }
}
