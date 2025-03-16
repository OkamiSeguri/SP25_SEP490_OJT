using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject
{
    public class Curriculum
    {
        public int CurriculumId { get; set; }
        public string? SubjectCode { get; set; }
        public string? SubjectName { get; set; }
        public int Credits { get; set; }
        public bool IsMandatory { get; set; } 

    }
}
