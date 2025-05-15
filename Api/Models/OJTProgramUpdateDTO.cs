using System.ComponentModel.DataAnnotations;

namespace FOMSOData.Models
{
    public class OJTProgramUpdateDTO
    {
        [Required]
        public string ProgramName { get; set; }

        public string? Description { get; set; }

        public string? Requirements { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public IFormFile? ImageFile { get; set; } // có thể null nếu không thay ảnh
    }
}
