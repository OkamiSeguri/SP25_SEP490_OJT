using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOMSDTO
{
    public class StudentGradeDTO
    {
        public int UserId { get; set; }
        public int CurriculumId { get; set; }
        public int Semester { get; set; }
        public decimal Grade { get; set; }
        public int IsPassed { get; set; }
    }
}
