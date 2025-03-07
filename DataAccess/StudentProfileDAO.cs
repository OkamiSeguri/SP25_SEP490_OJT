using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class StudentProfileDAO : SingletonBase<StudentProfileDAO>
    {
        public async Task<IEnumerable<StudentProfile>> GetStudentProfileAll()
        {
            return await _context.StudentProfiles.AsNoTracking().ToListAsync();
        }
        public async Task<StudentProfile> GetStudentProfile(int UserId, int StudentId)
        {
            var studentProfile = await _context.StudentProfiles.FirstOrDefaultAsync(c => c.UserId == UserId && c.StudentId == StudentId);
            if (studentProfile == null) return null; return studentProfile;
        }          
        public async Task<StudentProfile> GetStudentProfileById(int id)
        {
            var studentProfile = await _context.StudentProfiles.FirstOrDefaultAsync(c => c.StudentId == id);
            if (studentProfile == null) return null; return studentProfile;
        }      
        //public async Task<StudentProfile> GetStudentProfileByMajor(string major)
        //{
        //    var studentProfile = await _context.StudentProfiles.FirstOrDefaultAsync(c => c.Major == major);
        //    if (studentProfile == null) return null; return studentProfile;
        //}
        public async Task Create(StudentProfile studentProfile)
        {
            await _context.StudentProfiles.AddAsync(studentProfile);
            await _context.SaveChangesAsync();
        }
        public async Task Update(StudentProfile studentProfile)
        {
            var existingItem = await GetStudentProfileById(studentProfile.StudentId);
            if (existingItem != null)
            {
                _context.Entry(existingItem).CurrentValues.SetValues(studentProfile);
            }
            await _context.SaveChangesAsync();
        }
        public async Task Delete(int id)
        {
            var studentProfile = await GetStudentProfileById(id);
            if (studentProfile != null)
            {
                _context.StudentProfiles.Remove(studentProfile);
                await _context.SaveChangesAsync();
            }
        }

    }
}
