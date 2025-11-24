namespace EventManagement.Core.DTOs
{
    public class SpeakerBaseDto
    {
        public string Name { get; set; }
        public string? Biography { get; set; }
        public string? Email { get; set; }
    }

    public class SpeakerCreateDto : SpeakerBaseDto
    {
    }

    public class SpeakerUpdateDto : SpeakerBaseDto
    {
        public int SpeakerId { get; set; }
    }

    public class SpeakerResponseDto : SpeakerBaseDto
    {
        public int SpeakerId { get; set; }
    }
}
