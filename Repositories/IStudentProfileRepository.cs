using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IStudentProfileRepository
    {
        Task<IEnumerable<StudentProfile>> GetStudentProfileAll();
        Task<StudentProfile> GetStudentProfile(int UserId,int StudentId);
        Task<StudentProfile> GetStudentProfileById(int id);
        Task Create(StudentProfile studentProfile);
        Task Update(StudentProfile studentProfile);
        Task Delete(int id);
        Task<IEnumerable<Curriculum>> GetMandatorySubjectsAsync(int userId);
        Task<IEnumerable<Curriculum>> GetFailedMandatorySubjectsAsync(int userId);
    }
}
