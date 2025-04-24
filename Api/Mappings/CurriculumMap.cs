using BusinessObject;
using CsvHelper.Configuration;
using FOMSOData.Models;

namespace FOMSOData.Mappings
{
    public class CurriculumMap : ClassMap<CurriculumImportDTO>
    {
        public CurriculumMap()
        {
            Map(m => m.SubjectCode).Name("SubjectCode");
            Map(m => m.SubjectName).Name("SubjectName");
            Map(m => m.Credits).Name("Credits");
            Map(m => m.IsMandatory).Name("IsMandatory");
        }
    }

}
