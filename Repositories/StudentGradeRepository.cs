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
        public async Task<StudentGrade> GetGradeByUserId(int id)
        {
            return await StudentGradeDAO.Instance.GetGradeByUserId(id);
        }
        public async Task<StudentGrade> GetGrade(int UserId, int CuriculumId)
        {
            return await StudentGradeDAO.Instance.GetGrade(UserId, CuriculumId);
        }

        public async Task Create(StudentGrade studentGrade)
        {
            await StudentGradeDAO.Instance.Create(studentGrade);
        }

        public async Task Update(StudentGrade studentGrade)
        {
            await StudentGradeDAO.Instance.Update(studentGrade);
        }

        public async Task Delete(int OrderId, int ProductId)
        {
            await StudentGradeDAO.Instance.Delete(OrderId, ProductId);
        }
    }
}
