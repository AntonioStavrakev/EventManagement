using EventManagement.InfraStructure;

namespace EventManagement.Core.Models
{
    public class Event
    {
        public int EventId {  get; set; }
        public string? Title {  get; set; }
        public string? Description { get; set; }
        public DateTime Date {  get; set; }
        public string? Location { get; set; }
        public int SpeakerId { get; set; }


        public virtual Speaker Speaker { get; set; }

        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();

        public Event(int eventId, string title, string description, DateTime date, string location, int speakerId)
        {
            EventId = eventId;
            Title = title;
            Description = description;
            Date = date;
            Location = location;
            SpeakerId = speakerId;
            
        }

        public Event() { }
    }
}
