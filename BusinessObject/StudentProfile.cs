using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject
{
    public class StudentProfile
    {
        [Key]

        public int StudentId { get; set; }
        public int UserId { get; set; }
        public string Cohort { get; set; }
        public int CurriculumId { get; set; }
        public int? TotalCredits { get; set; }
        public int? DebtCredits { get; set; }
        public User? User { get; set; }
        [ForeignKey(nameof(Cohort))]
        public CohortCurriculum? CohortCurriculum { get; set; }


    }
}
