using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject
{
    public class Enterprise
    {
        public int EnterpriseId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public ICollection<OJTRegistration>? OJTRegistrations { get; set; }
        public virtual User User { get; set; }

    }
}
