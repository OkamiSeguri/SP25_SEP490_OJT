using BusinessObject;
using CsvHelper.Configuration;

namespace FOMSOData.Mappings
{
    public class CurriculumMap : ClassMap<Curriculum>
    {
        public CurriculumMap()
        {
            Map(m => m.CurriculumId).Name("CurriculumId");
            Map(m => m.SubjectCode).Name("SubjectCode");
            Map(m => m.SubjectName).Name("SubjectName");
            Map(m => m.Credits).Name("Credits");
            Map(m => m.IsMandatory).Name("IsMandatory");
        }
    }

}
