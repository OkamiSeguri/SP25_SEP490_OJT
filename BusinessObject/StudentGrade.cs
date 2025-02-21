using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject
{
    public class StudentGrade
    {
        public int UserId { get; set; }
        public int CurriculumId { get; set; }
        public int Semester { get; set; }
        public decimal Grade { get; set; }
        public int IsPassed { get; set; }
        public User? User { get; set; }
        public Curriculum? Curriculum { get; set; }


    }
}
