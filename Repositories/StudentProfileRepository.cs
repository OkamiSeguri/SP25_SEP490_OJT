using BusinessObject;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class StudentProfileRepository : IStudentProfileRepository
    {
        public async Task<IEnumerable<StudentProfile>> GetStudentProfileAll()
        {
            return await StudentProfileDAO.Instance.GetStudentProfileAll();
        }

        public async Task<StudentProfile> GetStudentProfile(int UserId,int StudentId)
        {
            return await StudentProfileDAO.Instance.GetStudentProfile(UserId,StudentId);
        }          
        public async Task<StudentProfile> GetStudentProfileById(int id)
        {
            return await StudentProfileDAO.Instance.GetStudentProfileById(id);
        }     
        //public async Task<StudentProfile> GetStudentProfileByMajor(string major)
        //{
        //    return await StudentProfileDAO.Instance.GetStudentProfileByMajor(major);
        //}

        public async Task Create(StudentProfile studentProfile)
        {
            await StudentProfileDAO.Instance.Create(studentProfile);
        }
        public async Task Update(StudentProfile studentProfile)
        {
            await StudentProfileDAO.Instance.Update(studentProfile);
        }
        public async Task Delete(int id)
        {
            await StudentProfileDAO.Instance.Delete(id);
        }
    }
}
