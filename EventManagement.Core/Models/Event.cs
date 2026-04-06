using EventManagement.InfraStructure;
using EventManagement.Services;

namespace EventManagement.Core.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public DateTime EndDate { get; set; }
        public string? Location { get; set; }
        public string Category { get; set; } = "Conference";
        public string Status { get; set; } = "Active";
        public int MaxCapacity { get; set; } = 50;
        public decimal TicketPrice { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public int SpeakerId { get; set; } = 1;
        public int OrganizerId { get; set; }

        public virtual Speaker Speaker { get; set; } = null!;
        public virtual User Organizer { get; set; } = null!;
        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();

        public Event(int eventId, string title, string description, DateTime date, string location, int speakerId)
        {
            EventId = eventId;
            Title = title;
            Description = description;
            Date = date;
            EndDate = date.AddHours(2);
            Location = location;
            SpeakerId = speakerId;
        }

        public Event()
        {
        }
    }
}
