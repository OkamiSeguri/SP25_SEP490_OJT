using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject
{
    public class OJTFeedback
    {
        [Key]
        public int FeedbackId { get; set; }
        public int OJTId { get; set; }
        public string GivenBy { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public OJTRegistration OJTRegistration { get; set; }
    }
}
