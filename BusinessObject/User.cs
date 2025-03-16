 using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject
{
    public class User
    {
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int Role { get; set; }
        public ICollection<OJTRegistration>? OJTRegistration { get; set; }
        public virtual Enterprise? Enterprise { get; set; }
        public StudentProfile? StudentProfile { get; set; }


    }
}
