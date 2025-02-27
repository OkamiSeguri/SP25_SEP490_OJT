using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOMSDTO
{
    public class StudentProfileDTO
    {
        public int StudentId { get; set; }
        public string Cohort { get; set; }
        public string Major { get; set; }
        public int TotalCredits { get; set; }
        public int DebtCredits { get; set; }
        public int UserId { get; set; }
    }
}
