using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject
{
    public class CohortCurriculum
    {
        [Key]
        //public int Id { get; set; }
        public int CohortCurriculumId { get; set; }

        public string? Cohort { get; set; }
        public int CurriculumId { get; set; }
        public int Semester { get; set; }
        public Curriculum? Curriculum { get; set; }
        public ICollection<StudentProfile> StudentProfiles { get; set; } = new List<StudentProfile>();
    }
}
