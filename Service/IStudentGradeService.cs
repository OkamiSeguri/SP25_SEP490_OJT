using FOMSDTO;

namespace FOMSService
{
    public interface IStudentGradeService
    {
        Task<IEnumerable<StudentGradeDTO>> GetGradesAll();
        Task<StudentGradeDTO> GetGrade(int userId, int curriculumId);
        Task Create(StudentGradeDTO studentGradeDTO);
        Task Update(StudentGradeDTO studentGradeDTO);
        Task Delete(int userId, int curriculumId);
    }
}
