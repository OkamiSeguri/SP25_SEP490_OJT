using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IStudentGradeRepository
    {
        Task<IEnumerable<StudentGrade>> GetGradesAll();
        Task<IEnumerable<StudentGrade>> GetGradeByUserId(int id);
        Task<StudentGrade> GetGrade(int UserId, int CurriculumId);
        Task UpdateStudentCredits(int UserId);
        Task Create(StudentGrade studentGrade);
        Task Update(StudentGrade studentGrade);
        Task Delete(int UserId, int CurriculumId);
        Task DeleteByUserId(int userId);
        Task<(List<int> MissingUserIds, List<int> MissingCurriculumIds)> ImportStudentGrades(IEnumerable<StudentGrade> studentGrade);
        Task CreateMultiple(IEnumerable<StudentGrade> studentGrades);

    }
}
