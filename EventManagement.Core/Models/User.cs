using EventManagement.Core.Models;
using EventManagement.InfraStructure;

namespace EventManagement.Services
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public ICollection<Event> CreatedEvents { get; set; } = new List<Event>();
        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();

        public string DisplayName => $"{FirstName} {LastName}".Trim();

        public User(int userId, string firstName, string lastName, string email)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }

        public User()
        {
        }
    }
}
