using System.Security.Claims;
using EventManagement.Core.Models;
using EventManagement.Core.ViewModels;
using EventManagement.InfraStructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Controllers;

public class EventsController : Controller
{
    private readonly EventDbContext _dbContext;

    public EventsController(EventDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Index(string? search, string? category, string scope = "all")
    {
        var currentUserId = GetCurrentUserId();

        var query = _dbContext.Events
            .Include(e => e.Organizer)
            .Include(e => e.Registrations)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = search.Trim().ToLower();
            query = query.Where(e =>
                (e.Title ?? string.Empty).ToLower().Contains(searchTerm) ||
                (e.Description ?? string.Empty).ToLower().Contains(searchTerm) ||
                (e.Location ?? string.Empty).ToLower().Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(e => e.Category == category);
        }

        if (scope == "mine" && currentUserId.HasValue)
        {
            query = query.Where(e => e.OrganizerId == currentUserId.Value);
        }
        else if (scope == "registered" && currentUserId.HasValue)
        {
            query = query.Where(e => e.Registrations.Any(r => r.UserId == currentUserId.Value));
        }
        else if (scope == "upcoming")
        {
            query = query.Where(e => e.Date >= DateTime.Now);
        }

        var events = await query
            .OrderBy(e => e.Date)
            .ToListAsync();

        ViewBag.Search = search;
        ViewBag.Category = category;
        ViewBag.Scope = scope;
        ViewBag.Categories = await _dbContext.Events
            .Select(e => e.Category)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();

        return View(events.Select(e => MapEvent(e, currentUserId)).ToList());
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> MyEvents()
    {
        var currentUserId = GetRequiredCurrentUserId();

        var events = await _dbContext.Events
            .Include(e => e.Organizer)
            .Include(e => e.Registrations)
            .Where(e => e.OrganizerId == currentUserId)
            .OrderBy(e => e.Date)
            .ToListAsync();

        return View(events.Select(e => MapEvent(e, currentUserId)).ToList());
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Schedule()
    {
        var currentUserId = GetRequiredCurrentUserId();

        var events = await _dbContext.Events
            .Include(e => e.Organizer)
            .Include(e => e.Registrations)
            .Where(e => e.Registrations.Any(r => r.UserId == currentUserId))
            .OrderBy(e => e.Date)
            .ToListAsync();

        return View(events.Select(e => MapEvent(e, currentUserId)).ToList());
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var currentUserId = GetCurrentUserId();

        var eventEntity = await _dbContext.Events
            .Include(e => e.Organizer)
            .Include(e => e.Registrations)
                .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(e => e.EventId == id);

        if (eventEntity == null)
        {
            return NotFound();
        }

        var model = new EventDetailsViewModel
        {
            Id = eventEntity.EventId,
            Title = eventEntity.Title ?? string.Empty,
            Description = eventEntity.Description,
            Category = eventEntity.Category,
            Status = eventEntity.Status,
            StartDate = eventEntity.Date,
            EndDate = eventEntity.EndDate,
            Location = eventEntity.Location,
            MaxCapacity = eventEntity.MaxCapacity,
            CurrentAttendees = eventEntity.Registrations.Count,
            TicketPrice = eventEntity.TicketPrice,
            CreatedOn = eventEntity.CreatedOn,
            OrganizerName = eventEntity.Organizer.DisplayName,
            OrganizerEmail = eventEntity.Organizer.Email,
            IsOwner = currentUserId == eventEntity.OrganizerId,
            IsRegistered = currentUserId.HasValue && eventEntity.Registrations.Any(r => r.UserId == currentUserId.Value),
            AvailableSpots = Math.Max(0, eventEntity.MaxCapacity - eventEntity.Registrations.Count),
            CanRegister = currentUserId.HasValue
                && currentUserId.Value != eventEntity.OrganizerId
                && eventEntity.Status != "Cancelled"
                && eventEntity.Date >= DateTime.Now
                && eventEntity.Registrations.Count < eventEntity.MaxCapacity,
            RecentAttendees = eventEntity.Registrations
                .OrderByDescending(r => r.RegistrationDate)
                .Take(8)
                .Select(r => new AttendeeListItemViewModel
                {
                    Id = r.RegistrationId,
                    EventId = r.EventId,
                    EventTitle = eventEntity.Title,
                    FirstName = r.User.FirstName,
                    LastName = r.User.LastName,
                    Email = r.User.Email,
                    RegistrationDate = r.RegistrationDate,
                    Status = r.Status,
                    TicketNumber = r.TicketNumber
                })
                .ToList()
        };

        return View(model);
    }

    [Authorize]
    [HttpGet]
    public IActionResult Create()
    {
        return View(new EventFormViewModel());
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EventFormViewModel model)
    {
        if (!ValidateEventForm(model))
        {
            return View(model);
        }

        var currentUserId = GetRequiredCurrentUserId();

        var eventEntity = new Event
        {
            Title = model.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim(),
            Category = model.Category,
            Status = model.Status,
            Date = model.StartDate,
            EndDate = model.EndDate,
            Location = model.Location.Trim(),
            MaxCapacity = model.MaxCapacity,
            TicketPrice = model.TicketPrice,
            OrganizerId = currentUserId,
            SpeakerId = 1,
            CreatedOn = DateTime.UtcNow
        };

        _dbContext.Events.Add(eventEntity);
        await _dbContext.SaveChangesAsync();

        TempData["Success"] = "Събитието е създадено успешно.";
        return RedirectToAction(nameof(MyEvents));
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var eventEntity = await GetOwnedEventAsync(id);
        if (eventEntity == null)
        {
            return NotFoundOrForbidden(id);
        }

        var model = new EventFormViewModel
        {
            Id = eventEntity.EventId,
            Title = eventEntity.Title ?? string.Empty,
            Description = eventEntity.Description,
            Category = eventEntity.Category,
            Status = eventEntity.Status,
            StartDate = eventEntity.Date,
            EndDate = eventEntity.EndDate,
            Location = eventEntity.Location ?? string.Empty,
            MaxCapacity = eventEntity.MaxCapacity,
            TicketPrice = eventEntity.TicketPrice
        };

        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EventFormViewModel model)
    {
        var eventEntity = await GetOwnedEventAsync(model.Id);
        if (eventEntity == null)
        {
            return NotFoundOrForbidden(model.Id);
        }

        if (!ValidateEventForm(model, eventEntity.Registrations.Count))
        {
            return View(model);
        }

        eventEntity.Title = model.Title.Trim();
        eventEntity.Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim();
        eventEntity.Category = model.Category;
        eventEntity.Status = model.Status;
        eventEntity.Date = model.StartDate;
        eventEntity.EndDate = model.EndDate;
        eventEntity.Location = model.Location.Trim();
        eventEntity.MaxCapacity = model.MaxCapacity;
        eventEntity.TicketPrice = model.TicketPrice;

        await _dbContext.SaveChangesAsync();
        TempData["Success"] = "Промените по събитието са запазени.";

        return RedirectToAction(nameof(Details), new { id = eventEntity.EventId });
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var currentUserId = GetRequiredCurrentUserId();
        var eventEntity = await _dbContext.Events
            .Include(e => e.Organizer)
            .Include(e => e.Registrations)
            .FirstOrDefaultAsync(e => e.EventId == id && e.OrganizerId == currentUserId);

        if (eventEntity == null)
        {
            return NotFoundOrForbidden(id);
        }

        return View(MapEvent(eventEntity, currentUserId));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var eventEntity = await GetOwnedEventAsync(id);
        if (eventEntity == null)
        {
            return NotFoundOrForbidden(id);
        }

        _dbContext.Events.Remove(eventEntity);
        await _dbContext.SaveChangesAsync();

        TempData["Success"] = "Събитието беше изтрито.";
        return RedirectToAction(nameof(MyEvents));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(int id)
    {
        var currentUserId = GetRequiredCurrentUserId();

        var eventEntity = await _dbContext.Events
            .Include(e => e.Registrations)
            .FirstOrDefaultAsync(e => e.EventId == id);

        if (eventEntity == null)
        {
            TempData["Error"] = "Събитието не беше намерено.";
            return RedirectToAction(nameof(Index));
        }

        if (eventEntity.OrganizerId == currentUserId)
        {
            TempData["Error"] = "Не е нужно да се регистрираш за събитие, което сам организираш.";
            return RedirectToAction(nameof(Details), new { id });
        }

        if (eventEntity.Status == "Cancelled" || eventEntity.Date < DateTime.Now)
        {
            TempData["Error"] = "Регистрацията за това събитие не е достъпна.";
            return RedirectToAction(nameof(Details), new { id });
        }

        if (eventEntity.Registrations.Any(r => r.UserId == currentUserId))
        {
            TempData["Error"] = "Вече си регистриран за това събитие.";
            return RedirectToAction(nameof(Details), new { id });
        }

        if (eventEntity.Registrations.Count >= eventEntity.MaxCapacity)
        {
            TempData["Error"] = "Няма свободни места за това събитие.";
            return RedirectToAction(nameof(Details), new { id });
        }

        _dbContext.Registrations.Add(new Registration
        {
            EventId = id,
            UserId = currentUserId,
            RegistrationDate = DateTime.UtcNow,
            Status = "Confirmed",
            TicketNumber = $"EV-{id:D4}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}"
        });

        await _dbContext.SaveChangesAsync();
        TempData["Success"] = "Успешно се регистрира за събитието.";

        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Leave(int id)
    {
        var currentUserId = GetRequiredCurrentUserId();
        var registration = await _dbContext.Registrations
            .FirstOrDefaultAsync(r => r.EventId == id && r.UserId == currentUserId);

        if (registration == null)
        {
            TempData["Error"] = "Нямаш активна регистрация за това събитие.";
            return RedirectToAction(nameof(Details), new { id });
        }

        _dbContext.Registrations.Remove(registration);
        await _dbContext.SaveChangesAsync();

        TempData["Success"] = "Регистрацията беше отменена.";
        return RedirectToAction(nameof(Details), new { id });
    }

    private bool ValidateEventForm(EventFormViewModel model, int currentAttendees = 0)
    {
        if (model.EndDate <= model.StartDate)
        {
            ModelState.AddModelError(nameof(model.EndDate), "Крайната дата трябва да е след началната.");
        }

        if (model.StartDate < DateTime.Now.AddMinutes(-1))
        {
            ModelState.AddModelError(nameof(model.StartDate), "Началната дата не може да бъде в миналото.");
        }

        if (model.MaxCapacity < currentAttendees)
        {
            ModelState.AddModelError(nameof(model.MaxCapacity), $"Капацитетът не може да е по-малък от текущите регистрации ({currentAttendees}).");
        }

        return ModelState.IsValid;
    }

    private async Task<Event?> GetOwnedEventAsync(int eventId)
    {
        var currentUserId = GetRequiredCurrentUserId();
        return await _dbContext.Events
            .Include(e => e.Registrations)
            .FirstOrDefaultAsync(e => e.EventId == eventId && e.OrganizerId == currentUserId);
    }

    private IActionResult NotFoundOrForbidden(int eventId)
    {
        var exists = _dbContext.Events.Any(e => e.EventId == eventId);
        if (exists)
        {
            TempData["Error"] = "Можеш да редактираш или триеш само събитията, които ти си създал.";
            return RedirectToAction(nameof(Details), new { id = eventId });
        }

        return NotFound();
    }

    private EventViewModel MapEvent(Event e, int? currentUserId)
    {
        var attendeeCount = e.Registrations.Count;

        return new EventViewModel
        {
            Id = e.EventId,
            Title = e.Title ?? string.Empty,
            Description = e.Description,
            Category = e.Category,
            Status = e.Status,
            StartDate = e.Date,
            EndDate = e.EndDate,
            Location = e.Location,
            MaxCapacity = e.MaxCapacity,
            CurrentAttendees = attendeeCount,
            TicketPrice = e.TicketPrice,
            CreatedOn = e.CreatedOn,
            OrganizerName = e.Organizer.DisplayName,
            OrganizerEmail = e.Organizer.Email,
            IsOwner = currentUserId.HasValue && currentUserId.Value == e.OrganizerId,
            IsRegistered = currentUserId.HasValue && e.Registrations.Any(r => r.UserId == currentUserId.Value),
            AvailableSpots = Math.Max(0, e.MaxCapacity - attendeeCount)
        };
    }

    private int? GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out var userId) ? userId : null;
    }

    private int GetRequiredCurrentUserId()
    {
        return GetCurrentUserId() ?? throw new InvalidOperationException("Current user is required.");
    }
}
