using FOMSDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FOMSService
{
    public class StudentProfileService : IStudentProfileService
    {
        private readonly HttpClient _httpClient;

        public StudentProfileService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<StudentProfileDTO>> GetStudentProfileAll()
            => await _httpClient.GetFromJsonAsync<IEnumerable<StudentProfileDTO>>("StudentProfile");
        public async Task<IEnumerable<StudentProfileDTO>> GetStudentProfileById(int id)
            => await _httpClient.GetFromJsonAsync<IEnumerable<StudentProfileDTO>>($"StudentProfile/profile/{id}");
               
        public async Task<IEnumerable<StudentProfileDTO>> GetStudentProfileByMajor(string major)
            => await _httpClient.GetFromJsonAsync<IEnumerable<StudentProfileDTO>>($"StudentProfile/major/{major}");


        public async Task Create(StudentProfileDTO studentProfile)
            => await _httpClient.PostAsJsonAsync("StudentProfile", studentProfile);

        public async Task Update(StudentProfileDTO studentProfile)
            => await _httpClient.PutAsJsonAsync($"StudentGrade/{studentProfile.UserId}", studentProfile);

        public async Task Delete(int id)
            => await _httpClient.DeleteAsync($"studentProfile/{id}");
    }
}
