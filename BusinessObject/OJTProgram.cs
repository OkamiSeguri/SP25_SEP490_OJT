using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject
{
    public class OJTProgram
    {
        [Key]
        public int ProgramId { get; set; }
        public int EnterpriseId { get; set; }
        public string ProgramName { get; set; }
        public string Description { get; set; }
        public string Requirements { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Enterprise Enterprise { get; set; }
        public ICollection<OJTRegistration> OJTRegistrations { get; set; }

    }
}
