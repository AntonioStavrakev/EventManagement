using EventManagement.Core.Models;

namespace EventManagement.InfraStructure
{
    public class Speaker
    {
        public int SpeakerID { get; set; }
        public string Name { get; set; }
        public string? Biography { get; set; }
        public string? Email { get; set; }

        public ICollection<Event> Events { get; set; } = new List<Event>();

        public Speaker(int speakerId, string name, string biography, string email) {
            SpeakerID = speakerId;
            Name = name;
            Biography = biography;
            Email = email;

        }
        public Speaker() { }
    }
}
