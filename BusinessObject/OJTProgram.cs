using System.ComponentModel.DataAnnotations;

namespace BusinessObject
{
    public class OJTProgram
    {
        [Key]
        public int ProgramId { get; set; }
        public int EnterpriseId { get; set; }
        public string? ProgramName { get; set; }
        public string? Description { get; set; }
        public string? Requirements { get; set; }
        public string? Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? ImageUrl { get; set; }
        public Enterprise? Enterprise { get; set; }
        public ICollection<OJTRegistration>? OJTRegistrations { get; set; }
        public ICollection<OJTFeedback>? OJTFeedbacks { get; set; }
    }
}
