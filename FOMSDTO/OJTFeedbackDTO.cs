using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOMSDTO
{
    public class OJTFeedbackDTO
    {
        public int FeedbackId { get; set; }
        public int OJTId { get; set; }
        public string GivenBy { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
