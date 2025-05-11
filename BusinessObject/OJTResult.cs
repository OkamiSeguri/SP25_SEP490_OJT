using System.ComponentModel.DataAnnotations;

namespace BusinessObject
{
    public class OJTResult
    {
        [Key]
        public int ResultId { get; set; }
        public int OJTId { get; set; }
        public int RegistrationId { get; set; }
        public int ProgramId { get; set; }
        public int EnterpriseId { get; set; }
        public decimal Score { get; set; }
        public string? Status { get; set; }
        public string? Comments { get; set; }
        public OJTRegistration? OJTRegistration { get; set; }
    }
}
