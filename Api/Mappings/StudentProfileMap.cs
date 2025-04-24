using CsvHelper.Configuration;
using FOMSOData.Models;

namespace FOMSOData.Mappings
{
    public class StudentProfileMap : ClassMap<StudentProfileImportDTO>
    {
        public StudentProfileMap()
        {
            Map(m => m.MSSV).Name("MSSV");
            Map(m => m.Cohort).Name("Cohort");

        }
    }
}
