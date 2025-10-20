using EventManagement.Core.Models;

namespace EventManagement.InfraStructure
{
    public class Speaker
    {
        public int SpeakerID { get; set; }
        public string Name { get; set; }
        public string Biography { get; set; }
        public string Email { get; set; }

        public ICollection<Event> Events { get; set; }
    }
}
