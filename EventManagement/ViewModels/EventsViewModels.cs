using System.ComponentModel.DataAnnotations;

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
    public IEnumerable<AttendeeListItemViewModel> RecentAttendees { get; set; } = Array.Empty<AttendeeListItemViewModel>();
}

public class DashboardViewModel
{
    public int TotalEvents { get; set; }
    public int ActiveEvents { get; set; }
    public int TotalAttendees { get; set; }
    public int UpcomingEvents { get; set; }
    public IEnumerable<EventViewModel> RecentEvents { get; set; } = Array.Empty<EventViewModel>();
}

public class EventFormViewModel
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    [Required]
    public string Category { get; set; } = "Other";
    public string Status { get; set; } = "Active";

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    public string Location { get; set; } = string.Empty;

    [Range(1, 100000)]
    public int MaxCapacity { get; set; }

    [Range(0, 100000)]
    public decimal TicketPrice { get; set; }
}

public class AttendeeListItemViewModel
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

public class AttendeesViewModel
{
    public int? EventId { get; set; }
    public string? EventTitle { get; set; }
    public IEnumerable<AttendeeListItemViewModel> Attendees { get; set; } = Array.Empty<AttendeeListItemViewModel>();
}

public class LoginViewModel
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }
}
