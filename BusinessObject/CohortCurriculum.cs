using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject
{
    public class CohortCurriculum
    {
        public string Cohort { get; set; }
        public int CurriculumId { get; set; }
        public int Semester { get; set; }
        public Curriculum Curriculum { get; set; }
    }
}
