using EventManagement.Core.Models;
using EventManagement.Services;
using System.ComponentModel.DataAnnotations;

namespace EventManagement.InfraStructure
{
    public class Registration
    {
        [Key]
        public int RegistrationId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        public DateTime RegistrationDate { get; set; } 

        
        public virtual User User { get; set; }
        public virtual Event Event { get; set; }

        public Registration() { }

        public Registration(int registrationId, int userId, int eventId)
        {
            RegistrationId = registrationId;
            UserId = userId;
            EventId = eventId;
            RegistrationDate = DateTime.Now;
        }
    }
}
