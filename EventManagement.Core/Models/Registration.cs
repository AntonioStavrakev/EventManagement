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

        [Required]
        public string Status { get; set; } = "Confirmed";

        [MaxLength(32)]
        public string? TicketNumber { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual Event Event { get; set; } = null!;

        public Registration()
        {
        }

        public Registration(int registrationId, int userId, int eventId)
        {
            RegistrationId = registrationId;
            UserId = userId;
            EventId = eventId;
            RegistrationDate = DateTime.UtcNow;
        }
    }
}
