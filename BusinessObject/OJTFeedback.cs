using System.ComponentModel.DataAnnotations;

namespace BusinessObject
{
    public class OJTFeedback
    {
        [Key]
        public int FeedbackId { get; set; }
        public int OJTId { get; set; }
        public int EnterpriseId { get; set; }
        public int ProgramId { get; set; }
        public string GivenBy { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public OJTProgram? OJTProgram { get; set; }
        public OJTRegistration? OJTRegistration { get; set; }

    }
}
