using CsvHelper.Configuration;
using FOMSOData.Models;

namespace FOMSOData.Mappings
{
    public class CohortCurriculumMap : ClassMap<CohortCurriculumImportDTO>
    {
        public CohortCurriculumMap()
        {
            Map(m => m.Cohort).Name("Cohort");
            Map(m => m.SubjectCode).Name("SubjectCode");
            Map(m => m.Semester).Name("Semester");
        }
    }
}
