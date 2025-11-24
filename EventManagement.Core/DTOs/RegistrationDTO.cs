namespace EventManagement.Core.DTOs
{
    public class RegistrationBaseDto
    {
        public DateTime RegistrationDate { get; set; }
    }

    public class RegistrationCreateDto : RegistrationBaseDto
    {
        public int UserId { get; set; }
        public int EventId { get; set; }
    }

    public class RegistrationUpdateDto : RegistrationBaseDto
    {
        public int RegistrationId { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }
    }

    public class RegistrationResponseDto : RegistrationBaseDto
    {
        public int RegistrationId { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }
        public string? UserName { get; set; }
        public string? EventTitle { get; set; }
    }
}
