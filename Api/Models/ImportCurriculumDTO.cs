namespace FOMSOData.Models
{
    public class ImportCurriculumDTO
    {
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public int Credits { get; set; }
        public bool IsMandatory    { get; set;  }
    }
}