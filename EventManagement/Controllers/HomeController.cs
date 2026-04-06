using System.Security.Claims;
using EventManagement.Core.ViewModels;
using EventManagement.InfraStructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    private readonly EventDbContext _dbContext;

    public HomeController(EventDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        var currentUserId = GetCurrentUserId();

        var events = await _dbContext.Events
            .Include(e => e.Organizer)
            .Include(e => e.Registrations)
            .OrderBy(e => e.Date)
            .ToListAsync();

        var recentEvents = events
            .Where(e => e.Date >= DateTime.Now)
            .Take(6)
            .Select(e => new EventViewModel
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
                CurrentAttendees = e.Registrations.Count,
                TicketPrice = e.TicketPrice,
                CreatedOn = e.CreatedOn,
                OrganizerName = e.Organizer.DisplayName,
                OrganizerEmail = e.Organizer.Email,
                IsOwner = currentUserId.HasValue && currentUserId.Value == e.OrganizerId,
                IsRegistered = currentUserId.HasValue && e.Registrations.Any(r => r.UserId == currentUserId.Value),
                AvailableSpots = Math.Max(0, e.MaxCapacity - e.Registrations.Count)
            })
            .ToList();

        var model = new DashboardViewModel
        {
            IsAuthenticated = User.Identity?.IsAuthenticated == true,
            UserFirstName = User.Identity?.IsAuthenticated == true ? User.Identity?.Name?.Split(' ').FirstOrDefault() : null,
            TotalEvents = events.Count,
            ActiveEvents = events.Count(e => e.Status == "Active"),
            TotalAttendees = events.Sum(e => e.Registrations.Count),
            UpcomingEvents = events.Count(e => e.Date >= DateTime.Now && e.Date <= DateTime.Now.AddDays(7)),
            MyEvents = currentUserId.HasValue ? events.Count(e => e.OrganizerId == currentUserId.Value) : 0,
            RegisteredEvents = currentUserId.HasValue ? events.Count(e => e.Registrations.Any(r => r.UserId == currentUserId.Value)) : 0,
            RecentEvents = recentEvents
        };

        return View(model);
    }

    private int? GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out var userId) ? userId : null;
    }
}
