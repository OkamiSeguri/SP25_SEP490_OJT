using System.ComponentModel.DataAnnotations;

namespace FOMSOData.Models
{
    public class OJTProgramCreateDTO
    {
        [Required]
        public int EnterpriseId { get; set; }

        [Required]
        public string ProgramName { get; set; }

        public string? Description { get; set; }
        public string? Requirements { get; set; }

        public string? Status { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public IFormFile ImageFile { get; set; }
    }
}
