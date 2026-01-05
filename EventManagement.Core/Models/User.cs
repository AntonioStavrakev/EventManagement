using EventManagement.InfraStructure;

namespace EventManagement.Services
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }

        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
        public User(int userId, string firstName, string lastNmae, string email) {
            UserId = userId;
            FirstName = firstName;
            LastName = lastNmae;
            Email = email;
        }
        public User() { }

    }
}
