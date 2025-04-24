using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject
{
    public class OJTResult
    {
        [Key]
        public int OJTId { get; set; }
        public int ResultId { get; set; }
        public decimal Score { get; set; }
        public string? Comments { get; set; }
        public OJTRegistration? OJTRegistration { get; set; }
    }
}
