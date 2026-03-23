using EventManagement.Core.DTOs;

namespace EventManagement.Core.ViewModels;

public class EventViewModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public string Category { get; set; } = "Other";
    public string Status { get; set; } = "Active";

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public string? Location { get; set; }

    public int MaxCapacity { get; set; }
    public int CurrentAttendees { get; set; }

    public decimal TicketPrice { get; set; }

    public DateTime CreatedOn { get; set; }

    public string OrganizerName { get; set; } = string.Empty;
    public string? OrganizerEmail { get; set; }
}

public class EventDetailsViewModel : EventViewModel
{
    public IEnumerable<AttendeeListItemViewModel> RecentAttendees { get; set; }
        = Array.Empty<AttendeeListItemViewModel>();
}

public class DashboardViewModel
{
    public int TotalEvents { get; set; }
    public int ActiveEvents { get; set; }
    public int TotalAttendees { get; set; }
    public int UpcomingEvents { get; set; }

    public IEnumerable<EventViewModel> RecentEvents { get; set; }
        = Array.Empty<EventViewModel>();
}