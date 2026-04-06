using EventManagement.Core.ViewModels;
using EventManagement.Models;

namespace EventManagement.Services;

public class InMemoryEventDemoStore : IEventDemoStore
{
    private readonly object _lock = new();
    private readonly List<EventRecord> _events;
    private int _nextEventId;
    private int _nextAttendeeId;

    public InMemoryEventDemoStore()
    {
        _events = Seed();
        _nextEventId = _events.Max(e => e.Id) + 1;
        _nextAttendeeId = _events.SelectMany(e => e.Attendees).DefaultIfEmpty(new AttendeeRecord { Id = 0 }).Max(a => a.Id) + 1;
    }

    public DashboardViewModel GetDashboard()
    {
        lock (_lock)
        {
            var events = GetEvents();
            return new DashboardViewModel
            {
                TotalEvents = events.Count,
                ActiveEvents = events.Count(e => e.Status.Equals("Active", StringComparison.OrdinalIgnoreCase)),
                TotalAttendees = _events.Sum(e => e.Attendees.Count),
                UpcomingEvents = events.Count(e => e.StartDate >= DateTime.Now && e.StartDate <= DateTime.Now.AddDays(7)),
                RecentEvents = events.OrderBy(e => e.StartDate).Take(5).ToList()
            };
        }
    }

    public List<EventViewModel> GetEvents()
    {
        lock (_lock)
        {
            return _events
                .OrderBy(e => e.StartDate)
                .Select(MapEvent)
                .ToList();
        }
    }

    public EventDetailsViewModel? GetEvent(int id)
    {
        lock (_lock)
        {
            var ev = _events.FirstOrDefault(e => e.Id == id);
            if (ev == null) return null;

            var details = new EventDetailsViewModel();
            CopyEvent(MapEvent(ev), details);
            details.RecentAttendees = ev.Attendees
                .OrderByDescending(a => a.RegistrationDate)
                .Take(5)
                .Select(MapAttendee)
                .ToList();
            return details;
        }
    }

    public EventFormViewModel? GetEventForm(int id)
    {
        lock (_lock)
        {
            var ev = _events.FirstOrDefault(e => e.Id == id);
            if (ev == null) return null;
            return new EventFormViewModel
            {
                Id = ev.Id,
                Title = ev.Title,
                Description = ev.Description,
                Category = ev.Category,
                Status = ev.Status,
                StartDate = ev.StartDate,
                EndDate = ev.EndDate,
                Location = ev.Location,
                MaxCapacity = ev.MaxCapacity,
                TicketPrice = ev.TicketPrice
            };
        }
    }

    public void CreateEvent(EventFormViewModel model, string organizerName, string organizerEmail)
    {
        lock (_lock)
        {
            _events.Add(new EventRecord
            {
                Id = _nextEventId++,
                Title = model.Title,
                Description = model.Description,
                Category = model.Category,
                Status = model.Status,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Location = model.Location,
                MaxCapacity = model.MaxCapacity,
                TicketPrice = model.TicketPrice,
                CreatedOn = DateTime.Now,
                OrganizerName = organizerName,
                OrganizerEmail = organizerEmail
            });
        }
    }

    public bool UpdateEvent(EventFormViewModel model)
    {
        lock (_lock)
        {
            var ev = _events.FirstOrDefault(e => e.Id == model.Id);
            if (ev == null) return false;
            ev.Title = model.Title;
            ev.Description = model.Description;
            ev.Category = model.Category;
            ev.Status = model.Status;
            ev.StartDate = model.StartDate;
            ev.EndDate = model.EndDate;
            ev.Location = model.Location;
            ev.MaxCapacity = model.MaxCapacity;
            ev.TicketPrice = model.TicketPrice;
            return true;
        }
    }

    public bool DeleteEvent(int id)
    {
        lock (_lock)
        {
            var ev = _events.FirstOrDefault(e => e.Id == id);
            if (ev == null) return false;
            _events.Remove(ev);
            return true;
        }
    }

    public AttendeesViewModel GetAttendees(int? eventId, string? search = null, string? statusFilter = null)
    {
        lock (_lock)
        {
            IEnumerable<AttendeeRecord> attendees = eventId.HasValue
                ? _events.FirstOrDefault(e => e.Id == eventId.Value)?.Attendees ?? new List<AttendeeRecord>()
                : _events.SelectMany(e => e.Attendees);

            if (!string.IsNullOrWhiteSpace(search))
            {
                attendees = attendees.Where(a =>
                    ($"{a.FirstName} {a.LastName}").Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    a.Email.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                attendees = attendees.Where(a => a.Status.Equals(statusFilter, StringComparison.OrdinalIgnoreCase));
            }

            var ev = eventId.HasValue ? _events.FirstOrDefault(e => e.Id == eventId.Value) : null;

            return new AttendeesViewModel
            {
                EventId = eventId,
                EventTitle = ev?.Title,
                Attendees = attendees
                    .OrderByDescending(a => a.RegistrationDate)
                    .Select(MapAttendee)
                    .ToList()
            };
        }
    }

    public bool RemoveAttendee(int id)
    {
        lock (_lock)
        {
            foreach (var ev in _events)
            {
                var attendee = ev.Attendees.FirstOrDefault(a => a.Id == id);
                if (attendee != null)
                {
                    ev.Attendees.Remove(attendee);
                    return true;
                }
            }

            return false;
        }
    }

    public bool ValidateUser(string username, string password, out string displayName, out string email, out string role)
    {
        displayName = string.Empty;
        email = string.Empty;
        role = string.Empty;

        var users = new[]
        {
            new { Username = "admin", Password = "admin123", Display = "Admin User", Email = "admin@eventmanagement.local", Role = "Admin" },
            new { Username = "user", Password = "user123", Display = "Demo User", Email = "user@eventmanagement.local", Role = "User" }
        };

        var match = users.FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && u.Password == password);

        if (match == null) return false;

        displayName = match.Display;
        email = match.Email;
        role = match.Role;
        return true;
    }

    private static EventViewModel MapEvent(EventRecord ev) => new()
    {
        Id = ev.Id,
        Title = ev.Title,
        Description = ev.Description,
        Category = ev.Category,
        Status = ev.Status,
        StartDate = ev.StartDate,
        EndDate = ev.EndDate,
        Location = ev.Location,
        MaxCapacity = ev.MaxCapacity,
        CurrentAttendees = ev.Attendees.Count,
        TicketPrice = ev.TicketPrice,
        CreatedOn = ev.CreatedOn,
        OrganizerName = ev.OrganizerName,
        OrganizerEmail = ev.OrganizerEmail
    };

    private static AttendeeListItemViewModel MapAttendee(AttendeeRecord a) => new()
    {
        Id = a.Id,
        EventId = a.EventId,
        EventTitle = a.EventTitle,
        FirstName = a.FirstName,
        LastName = a.LastName,
        Email = a.Email,
        RegistrationDate = a.RegistrationDate,
        Status = a.Status,
        TicketNumber = a.TicketNumber
    };

    private static void CopyEvent(EventViewModel source, EventViewModel target)
    {
        target.Id = source.Id;
        target.Title = source.Title;
        target.Description = source.Description;
        target.Category = source.Category;
        target.Status = source.Status;
        target.StartDate = source.StartDate;
        target.EndDate = source.EndDate;
        target.Location = source.Location;
        target.MaxCapacity = source.MaxCapacity;
        target.CurrentAttendees = source.CurrentAttendees;
        target.TicketPrice = source.TicketPrice;
        target.CreatedOn = source.CreatedOn;
        target.OrganizerName = source.OrganizerName;
        target.OrganizerEmail = source.OrganizerEmail;
    }

    private List<EventRecord> Seed()
    {
        var event1 = new EventRecord
        {
            Id = 1,
            Title = "Tech Innovation Summit 2026",
            Description = "Годишна конференция за нови технологии, AI и cloud решения.",
            Category = "Conference",
            Status = "Active",
            StartDate = DateTime.Now.AddDays(3).Date.AddHours(10),
            EndDate = DateTime.Now.AddDays(3).Date.AddHours(18),
            Location = "Sofia Tech Park",
            MaxCapacity = 250,
            TicketPrice = 49,
            CreatedOn = DateTime.Now.AddDays(-10),
            OrganizerName = "Admin User",
            OrganizerEmail = "admin@eventmanagement.local"
        };

        var event2 = new EventRecord
        {
            Id = 2,
            Title = "Frontend Best Practices Workshop",
            Description = "Практически workshop за ASP.NET MVC, clean UI и presentation-ready dashboards.",
            Category = "Workshop",
            Status = "Active",
            StartDate = DateTime.Now.AddDays(7).Date.AddHours(14),
            EndDate = DateTime.Now.AddDays(7).Date.AddHours(18),
            Location = "Hall B",
            MaxCapacity = 80,
            TicketPrice = 0,
            CreatedOn = DateTime.Now.AddDays(-6),
            OrganizerName = "Demo User",
            OrganizerEmail = "user@eventmanagement.local"
        };

        event1.Attendees.AddRange(new[]
        {
            NewAttendee(1, event1, "Иван", "Петров", "ivan.petrov@example.com", "Confirmed"),
            NewAttendee(2, event1, "Мария", "Георгиева", "maria.georgieva@example.com", "Confirmed"),
            NewAttendee(3, event1, "Стефан", "Илиев", "stefan.iliev@example.com", "Pending")
        });

        event2.Attendees.AddRange(new[]
        {
            NewAttendee(4, event2, "Елена", "Стоянова", "elena.stoyanova@example.com", "Confirmed"),
            NewAttendee(5, event2, "Никола", "Димитров", "nikola.dimitrov@example.com", "Cancelled")
        });

        return new List<EventRecord> { event1, event2 };
    }

    private AttendeeRecord NewAttendee(int id, EventRecord ev, string firstName, string lastName, string email, string status)
    {
        return new AttendeeRecord
        {
            Id = id,
            EventId = ev.Id,
            EventTitle = ev.Title,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            RegistrationDate = DateTime.Now.AddDays(-id),
            Status = status,
            TicketNumber = $"TKT-{ev.Id:D2}-{id:D4}"
        };
    }
}
