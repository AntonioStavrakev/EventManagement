using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement.Core.DTOs
{
    public class EventBaseDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public string? Location { get; set; }
    }

    public class EventCreateDto : EventBaseDto
    {
        public int SpeakerId { get; set; }
    }

    public class EventUpdateDto : EventBaseDto
    {
        public int EventId { get; set; }
        public int SpeakerId { get; set; }
    }

    public class EventResponseDto : EventBaseDto
    {
        public int EventId { get; set; }
        public int SpeakerId { get; set; }
        public string SpeakerName { get; set; }
    }
}
