using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject
{
    public class StudentProfile
    {
        [Key]

        public int StudentId { get; set; }
        public int CohortCurriculumId { get; set; }
        public int? TotalCredits { get; set; }
        public int? DebtCredits { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public CohortCurriculum? CohortCurriculum { get; set; }


    }
}
