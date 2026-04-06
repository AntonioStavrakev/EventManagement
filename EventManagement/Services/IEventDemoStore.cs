using EventManagement.Core.ViewModels;

namespace EventManagement.Services;

public interface IEventDemoStore
{
    DashboardViewModel GetDashboard();
    List<EventViewModel> GetEvents();
    EventDetailsViewModel? GetEvent(int id);
    EventFormViewModel? GetEventForm(int id);
    void CreateEvent(EventFormViewModel model, string organizerName, string organizerEmail);
    bool UpdateEvent(EventFormViewModel model);
    bool DeleteEvent(int id);
    AttendeesViewModel GetAttendees(int? eventId, string? search = null, string? statusFilter = null);
    bool RemoveAttendee(int id);
    bool ValidateUser(string username, string password, out string displayName, out string email, out string role);
}
