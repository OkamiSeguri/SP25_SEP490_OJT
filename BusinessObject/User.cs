namespace BusinessObject
{
    public class User
    {
        public int UserId { get; set; }
        public string? MSSV { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Role { get; set; }
        public bool IsDeleted { get; set; } = false;
        public virtual Enterprise? Enterprise { get; set; }
        public StudentProfile? StudentProfile { get; set; }
    }
}
