using BusinessObject;
using CsvHelper.Configuration;
using FOMSOData.Models;

namespace FOMSOData.Mappings
{
    public class StudentGradeMap : ClassMap<StudentGradeImportDTO>
    {
        public StudentGradeMap()
        {
            Map(m => m.MSSV).Name("MSSV");
            Map(m => m.SubjectCode).Name("SubjectCode");
            Map(m => m.Semester).Name("Semester");
            Map(m => m.Grade).Name("Grade");
        }
    }
}
