using BusinessObject;
using CsvHelper.Configuration;

namespace FOMSOData.Mappings
{
    public class StudentGradeMap : ClassMap<StudentGrade>
    {
        public StudentGradeMap()
        {
            Map(m => m.UserId).Name("UserId");
            Map(m => m.CurriculumId).Name("CurriculumId");
            Map(m => m.Semester).Name("Semester");
            Map(m => m.Grade).Name("Grade");
            Map(m => m.IsPassed).Name("IsPassed");
        }
    }
}
