namespace EventManagement.Models;

public class AttendeeRecord
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public string? EventTitle { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public string Status { get; set; } = "Confirmed";
    public string? TicketNumber { get; set; }
}
