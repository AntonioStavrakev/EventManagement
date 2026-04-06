namespace EventManagement.Models;

public class EventRecord
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = "Other";
    public string Status { get; set; } = "Active";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public int MaxCapacity { get; set; }
    public decimal TicketPrice { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public string OrganizerName { get; set; } = "Admin User";
    public string OrganizerEmail { get; set; } = "admin@eventmanagement.local";
    public List<AttendeeRecord> Attendees { get; set; } = new();
}
