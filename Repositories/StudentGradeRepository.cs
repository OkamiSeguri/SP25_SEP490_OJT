using BusinessObject;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class StudentGradeRepository : IStudentGradeRepository
    {
        public async Task<IEnumerable<StudentGrade>> GetGradesAll()
        {
            return await StudentGradeDAO.Instance.GetGradesAll();
        }
        public async Task<IEnumerable<StudentGrade>> GetGradeByUserId(int id)
        {
            return await StudentGradeDAO.Instance.GetGradeByUserId(id);
        }
        public async Task<StudentGrade> GetGrade(int UserId, int CuriculumId)
        {
            return await StudentGradeDAO.Instance.GetGrade(UserId, CuriculumId);
        }
        public async Task UpdateStudentCredits(int UserId)
        {
            await StudentGradeDAO.Instance.UpdateStudentCredits(UserId);
        }


        public async Task Create(StudentGrade studentGrade)
        {
            await StudentGradeDAO.Instance.Create(studentGrade);
            await StudentGradeDAO.Instance.UpdateStudentCredits(studentGrade.UserId);

        }

        public async Task Update(StudentGrade studentGrade)
        {
            await StudentGradeDAO.Instance.Update(studentGrade);
            await UpdateStudentCredits(studentGrade.UserId);
        }

        public async Task Delete(int UserId, int CurriculumId)
        {
            await StudentGradeDAO.Instance.Delete(UserId, CurriculumId);
            await UpdateStudentCredits(UserId);

        }

        public async Task<(List<int> MissingUserIds, List<int> MissingCurriculumIds)> ImportStudentGrades(IEnumerable<StudentGrade> grades)
        {
            return await StudentGradeDAO.Instance.ImportStudentGradesAsync(grades);
        }



    }
}
