namespace EventManagement.Core.ViewModels;

public class EventFormViewModel
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
}