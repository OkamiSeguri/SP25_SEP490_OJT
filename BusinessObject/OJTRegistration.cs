using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject
{
    public class OJTRegistration
    {
        [Key]

        public int OJTId { get; set; }
        public int StudentId { get; set; }
        public int EnterpriseId { get; set; }
        public int ProgramId { get; set; }
        public string Status { get; set; }
        public ICollection<OJTFeedback> OJTFeedbacks { get; set; }
        public ICollection<OJTResult> OJTResults { get; set; }
        public OJTProgram OJTProgram { get; set; }
        public Enterprise Enterprise { get; set; }
        public User User { get; set; }


    }

}
