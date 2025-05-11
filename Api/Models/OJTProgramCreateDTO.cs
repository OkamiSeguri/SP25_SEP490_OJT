namespace FOMSOData.Models
{
    public class OJTProgramCreateDTO
    {
        public int EnterpriseId { get; set; }
        public string? ProgramName { get; set; }
        public string? Description { get; set; }
        public string? Requirements { get; set; }
        public string? Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
