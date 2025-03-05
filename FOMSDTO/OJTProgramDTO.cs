using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOMSDTO
{
    public class OJTProgramDTO
    {
        public int ProgramId { get; set; }
        public int EnterpriseId { get; set; }
        public string ProgramName { get; set; }
        public string Description { get; set; }
        public string Requirements { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
