using FOMSDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOMSService
{
    public interface IStudentProfileService
    {
        Task<IEnumerable<StudentProfileDTO>> GetStudentProfileAll();
        Task<IEnumerable<StudentProfileDTO>> GetStudentProfileById(int id);
        Task<IEnumerable<StudentProfileDTO>> GetStudentProfileByMajor(string major);
        Task Create(StudentProfileDTO studentProfile);
        Task Update(StudentProfileDTO studentProfile);
        Task Delete(int id );
    }
}