using FOMSDTO;
using System.Net.Http.Json;

namespace FOMSService
{
    public class StudentGradeService : IStudentGradeService
    {
        private readonly HttpClient _httpClient;

        public StudentGradeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<StudentGradeDTO>> GetGradesAll()
            => await _httpClient.GetFromJsonAsync<IEnumerable<StudentGradeDTO>>("StudentGrade");
        public async Task<StudentGradeDTO> GetUserById(int id)
            => await _httpClient.GetFromJsonAsync<StudentGradeDTO>($"StudentGrade/{id}");

        public async Task<StudentGradeDTO> GetGrade(int userId, int curriculumId)
            => await _httpClient.GetFromJsonAsync<StudentGradeDTO>($"StudentGrade/{userId}/{curriculumId}");

        public async Task Create(StudentGradeDTO studentGradeDTO)
            => await _httpClient.PostAsJsonAsync("StudentGrade", studentGradeDTO);

        public async Task Update(StudentGradeDTO studentGradeDTO)
            => await _httpClient.PutAsJsonAsync($"StudentGrade/{studentGradeDTO.UserId}/{studentGradeDTO.CurriculumId}", studentGradeDTO);

        public async Task Delete(int userId, int curriculumId)
            => await _httpClient.DeleteAsync($"StudentGrade/{userId}/{curriculumId}");
    }
}
