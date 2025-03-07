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
        public async Task UpdateStudentCredits(int userId)
        {
            var studentProfile = await _context.StudentProfiles.FirstOrDefaultAsync(s => s.UserId == userId);
            if (studentProfile == null)
            {
                // Nếu chưa có, tạo mới StudentProfile
                studentProfile = new StudentProfile
                {
                    UserId = userId,
                    TotalCredits = 0,
                    DebtCredits = 0
                };

                _context.StudentProfiles.Add(studentProfile);
            }
            // Lấy tất cả điểm của sinh viên này
            var grades = await _context.StudentGrades.Where(g => g.UserId == userId).ToListAsync();

            // Lấy số tín chỉ của các môn học
            var curriculumCredits = await _context.Curriculums.ToDictionaryAsync(c => c.CurriculumId, c => c.Credits);

            int totalCredits = 0;
            int debtCredits = 0;

            foreach (var grade in grades)
            {
                if (curriculumCredits.TryGetValue(grade.CurriculumId, out int credits))
                {
                    if (grade.Grade >= 5)
                        totalCredits += credits; // Đậu -> Cộng vào TotalCredits
                    else
                        debtCredits += credits; // Rớt -> Cộng vào DebtCredits
                }
            }

            // Cập nhật bảng StudentProfiles
            studentProfile.TotalCredits = totalCredits;
            studentProfile.DebtCredits = debtCredits;

            await _context.SaveChangesAsync();
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
            await UpdateStudentCredits(studentGrade.UserId);

        }
        public async Task Update(StudentGrade studentGrade)
        {
            var existingItem = await GetGrade(studentGrade.UserId,studentGrade.CurriculumId);
            if (existingItem != null)
            {
                _context.Entry(existingItem).CurrentValues.SetValues(studentGrade);
            }
            await _context.SaveChangesAsync();
            await UpdateStudentCredits(studentGrade.UserId);

        }
        public async Task Delete(int UserId, int CurriculumId)
        {
            var grade = await GetGrade(UserId,CurriculumId);
            if (grade != null)
            {
                _context.StudentGrades.Remove(grade);
                await _context.SaveChangesAsync();
                await UpdateStudentCredits(UserId);

            }
        }
    }
}
