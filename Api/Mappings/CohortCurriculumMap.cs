using BusinessObject;
using CsvHelper.Configuration;

namespace FOMSOData.Mappings
{
    public class CohortCurriculumMap : ClassMap<CohortCurriculum>
    {
        public CohortCurriculumMap()
        {
            Map(m => m.CohortCurriculumId).Name("CohortCurriculumId");
            Map(m => m.Cohort).Name("Cohort");
            Map(m => m.CurriculumId).Name("CurriculumId");
            Map(m => m.Semester).Name("Semester");
        }
    }
}
