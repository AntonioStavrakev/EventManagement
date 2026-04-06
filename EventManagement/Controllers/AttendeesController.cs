using System.Security.Claims;
using System.Text;
using EventManagement.Core.ViewModels;
using EventManagement.InfraStructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Controllers;

[Authorize]
public class AttendeesController : Controller
{
    private readonly EventDbContext _dbContext;

    public AttendeesController(EventDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int eventId, string? search, string? statusFilter)
    {
        var currentUserId = GetRequiredCurrentUserId();

        var eventEntity = await _dbContext.Events
            .Include(e => e.Registrations)
                .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(e => e.EventId == eventId && e.OrganizerId == currentUserId);

        if (eventEntity == null)
        {
            TempData["Error"] = "Имаш достъп само до участниците в собствените си събития.";
            return RedirectToAction("MyEvents", "Events");
        }

        var registrations = eventEntity.Registrations.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = search.Trim().ToLower();
            registrations = registrations.Where(r =>
                r.User.FirstName.ToLower().Contains(searchTerm) ||
                r.User.LastName.ToLower().Contains(searchTerm) ||
                r.User.Email.ToLower().Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(statusFilter))
        {
            registrations = registrations.Where(r => r.Status == statusFilter);
        }

        ViewBag.Search = search;
        ViewBag.StatusFilter = statusFilter;
        ViewBag.TotalPages = 1;
        ViewBag.CurrentPage = 1;

        var model = new AttendeesViewModel
        {
            EventId = eventEntity.EventId,
            EventTitle = eventEntity.Title,
            Attendees = registrations
                .OrderByDescending(r => r.RegistrationDate)
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int id)
    {
        var currentUserId = GetRequiredCurrentUserId();

        var registration = await _dbContext.Registrations
            .Include(r => r.Event)
            .FirstOrDefaultAsync(r => r.RegistrationId == id);

        if (registration == null)
        {
            TempData["Error"] = "Участникът не беше намерен.";
            return RedirectToAction("MyEvents", "Events");
        }

        if (registration.Event.OrganizerId != currentUserId)
        {
            TempData["Error"] = "Можеш да премахваш участници само от собствените си събития.";
            return RedirectToAction("Details", "Events", new { id = registration.EventId });
        }

        var eventId = registration.EventId;
        _dbContext.Registrations.Remove(registration);
        await _dbContext.SaveChangesAsync();

        TempData["Success"] = "Участникът беше премахнат.";
        return RedirectToAction(nameof(Index), new { eventId });
    }

    [HttpGet]
    public async Task<IActionResult> ExportCsv(int eventId)
    {
        var currentUserId = GetRequiredCurrentUserId();

        var eventEntity = await _dbContext.Events
            .Include(e => e.Registrations)
                .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(e => e.EventId == eventId && e.OrganizerId == currentUserId);

        if (eventEntity == null)
        {
            TempData["Error"] = "Можеш да експортираш само участниците на собствените си събития.";
            return RedirectToAction("MyEvents", "Events");
        }

        var builder = new StringBuilder();
        builder.AppendLine("FirstName,LastName,Email,RegistrationDate,Status,TicketNumber");

        foreach (var attendee in eventEntity.Registrations.OrderBy(r => r.RegistrationDate))
        {
            builder.AppendLine(string.Join(",",
                Escape(attendee.User.FirstName),
                Escape(attendee.User.LastName),
                Escape(attendee.User.Email),
                attendee.RegistrationDate.ToString("yyyy-MM-dd HH:mm"),
                Escape(attendee.Status),
                Escape(attendee.TicketNumber ?? string.Empty)));
        }

        var bytes = Encoding.UTF8.GetBytes(builder.ToString());
        var fileName = $"attendees-{eventEntity.EventId}-{DateTime.Now:yyyyMMddHHmm}.csv";
        return File(bytes, "text/csv", fileName);
    }

    private static string Escape(string value)
    {
        return $"\"{value.Replace("\"", "\"\"")}\"";
    }

    private int GetRequiredCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out var userId)
            ? userId
            : throw new InvalidOperationException("Current user is required.");
    }
}
