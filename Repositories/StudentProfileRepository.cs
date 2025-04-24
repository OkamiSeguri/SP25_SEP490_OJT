using BusinessObject;
using DataAccess;
using Microsoft.EntityFrameworkCore;
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
        public async Task<List<StudentProfile>> GetStudentProfilesByUserIds(List<int> userIds)
        {
            return await StudentProfileDAO.Instance.GetStudentProfilesByUserIds(userIds);
        }
        public async Task<StudentProfile> GetStudentProfileByUserId(int id)
        {
            return await StudentProfileDAO.Instance.GetStudentProfileByUserId(id);
        }     

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
        public async Task DeleteByUserId(int UserId)
        {
            await StudentProfileDAO.Instance.DeleteByUserId(UserId);

        }
        public async Task<IEnumerable<Curriculum>> GetMandatorySubjectsAsync(int userId)
        {
            return await StudentProfileDAO.Instance.GetMandatorySubjectsAsync(userId);
        }

        public async Task<IEnumerable<Curriculum>> GetFailedMandatorySubjectsAsync(int userId)
        {
            return await StudentProfileDAO.Instance.GetFailedMandatorySubjectsAsync(userId);
        }
        public async Task<(List<int> MissingUserIds, List<string> MissingCohorts)> ImportStudentProfilesAsync(IEnumerable<StudentProfile> studentProfiles)
        {
            return await StudentProfileDAO.Instance.ImportStudentProfilesAsync(studentProfiles);
        }
    }
}
