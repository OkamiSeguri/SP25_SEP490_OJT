using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess
{
    public class StudentGradeDAO : SingletonBase<StudentGradeDAO>
    {


        public async Task<IEnumerable<StudentGrade>> GetGradesAll()
        {
            return await _context.StudentGrades.ToListAsync();
        }
        public async Task<IEnumerable<StudentGrade>> GetGradeByUserId(int id)
        {
            var grades = await _context.StudentGrades
                .Where(c => c.UserId == id)
                .ToListAsync();  

            return grades; 
        }

        public async Task<StudentGrade> GetGrade(int UserId, int CurriculumId)
        {
            var grade = await _context.StudentGrades.FirstOrDefaultAsync(c => c.UserId == UserId && c.CurriculumId == CurriculumId);
            if (grade == null) return null; return grade;

        }
        public async Task Create(StudentGrade studentGrade)
        {
            await _context.StudentGrades.AddAsync(studentGrade);
            await _context.SaveChangesAsync();
        }
        public async Task Update(StudentGrade studentGrade)
        {
            var existingItem = await GetGrade(studentGrade.UserId,studentGrade.CurriculumId);
            if (existingItem != null)
            {
                _context.Entry(existingItem).CurrentValues.SetValues(studentGrade);
            }
            await _context.SaveChangesAsync();
        }
        public async Task Delete(int UserId, int CurriculumId)
        {
            var grade = await GetGrade(UserId,CurriculumId);
            if (grade != null)
            {
                _context.StudentGrades.Remove(grade);
                await _context.SaveChangesAsync();
            }
        }
    }
}
