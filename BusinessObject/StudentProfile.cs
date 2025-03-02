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
        public string Cohort { get; set; }
        public string Major { get; set; }
        public int TotalCredits { get; set; }
        public int DebtCredits { get; set; }
        public User User { get; set; }
    }
}
