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
                        totalCredits += credits; 
                    else
                        debtCredits += credits; 
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
        public async Task DeleteByUserId(int userId)
        {
            var grades = _context.StudentGrades.Where(g => g.UserId == userId);
            _context.StudentGrades.RemoveRange(grades);
            await _context.SaveChangesAsync();
        }

        public async Task<(List<int> MissingUserIds, List<int> MissingCurriculumIds)> ImportStudentGradesAsync(IEnumerable<StudentGrade> grades)
        {
            var existingCurriculumIds = await _context.Curriculums.Select(c => c.CurriculumId).ToHashSetAsync();
            var existingUserIds = await _context.Users.Select(u => u.UserId).ToHashSetAsync();

            List<int> missingUserIds = new List<int>();
            List<int> missingCurriculumIds = new List<int>();

            var existingGrades = await _context.StudentGrades
                .Where(g => grades.Select(sg => sg.UserId).Contains(g.UserId) &&
                            grades.Select(sg => sg.CurriculumId).Contains(g.CurriculumId))
                .ToDictionaryAsync(g => new { g.UserId, g.CurriculumId });

            var gradesToAdd = new List<StudentGrade>();

            foreach (var grade in grades)
            {
                if (!existingUserIds.Contains(grade.UserId))
                {
                    missingUserIds.Add(grade.UserId);
                    continue;
                }

                if (!existingCurriculumIds.Contains(grade.CurriculumId))
                {
                    missingCurriculumIds.Add(grade.CurriculumId);
                    continue;
                }

                var key = new { grade.UserId, grade.CurriculumId };

                if (existingGrades.TryGetValue(key, out var existingGrade))
                {
                    _context.Entry(existingGrade).State = EntityState.Detached;

                    existingGrade.Grade = grade.Grade;
                    existingGrade.Semester = grade.Semester;

                    _context.StudentGrades.Attach(existingGrade);
                    _context.Entry(existingGrade).State = EntityState.Modified;
                }
                else
                {
                    gradesToAdd.Add(grade);
                }
            }

            if (gradesToAdd.Any())
            {
                await _context.StudentGrades.AddRangeAsync(gradesToAdd);
            }

            await _context.SaveChangesAsync();
            var affectedUserIds = grades
       .Where(g => !missingUserIds.Contains(g.UserId))
       .Select(g => g.UserId)
       .Distinct();

            foreach (var userId in affectedUserIds)
            {
                await UpdateStudentCredits(userId);
            }
            return (missingUserIds, missingCurriculumIds);
        }


        public async Task CreateMultiple(IEnumerable<StudentGrade> studentGrades)
        {
            await _context.StudentGrades.AddRangeAsync(studentGrades);
            await _context.SaveChangesAsync();

            var userIds = studentGrades.Select(g => g.UserId).Distinct();
            foreach (var userId in userIds)
            {
                await UpdateStudentCredits(userId);
            }
        }



    }
}
