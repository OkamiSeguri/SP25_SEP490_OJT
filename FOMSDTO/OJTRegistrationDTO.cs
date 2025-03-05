using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOMSDTO
{
    public class OJTRegistrationDTO
    {
        public int OJTId { get; set; }
        public int StudentId { get; set; }
        public int EnterpriseId { get; set; }
        public int ProgramId { get; set; }
        public string Status { get; set; }
    }
}
