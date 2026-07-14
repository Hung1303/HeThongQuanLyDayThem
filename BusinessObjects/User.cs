using Core.Base;

namespace BusinessObjects
{
    public class User : BaseEntity
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Fullname { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public Role UserRole { get; set; }
        public AccountStatus AccountStatus { get; set; } = 0;

    }
}
