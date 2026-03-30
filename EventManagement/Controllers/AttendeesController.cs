using EventManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Controllers;

[Authorize]
public class AttendeesController : Controller
{
    private readonly IEventDemoStore _store;

    public AttendeesController(IEventDemoStore store)
    {
        _store = store;
    }

    public IActionResult Index(int? eventId, string? search, string? statusFilter)
    {
        ViewBag.Search = search;
        ViewBag.StatusFilter = statusFilter;
        ViewBag.CurrentPage = 1;
        ViewBag.TotalPages = 1;
        return View(_store.GetAttendees(eventId, search, statusFilter));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public IActionResult Remove(int id)
    {
        _store.RemoveAttendee(id);
        TempData["Success"] = "Участникът беше премахнат.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult ExportCsv(int? eventId)
    {
        var model = _store.GetAttendees(eventId);
        var lines = new List<string> { "FirstName,LastName,Email,EventTitle,RegistrationDate,Status,TicketNumber" };
        lines.AddRange(model.Attendees.Select(a =>
            string.Join(',', Escape(a.FirstName), Escape(a.LastName), Escape(a.Email), Escape(a.EventTitle), a.RegistrationDate.ToString("yyyy-MM-dd"), Escape(a.Status), Escape(a.TicketNumber))));
        var bytes = System.Text.Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, lines));
        return File(bytes, "text/csv", "attendees.csv");
    }

    private static string Escape(string? value) => $"\"{(value ?? string.Empty).Replace("\"", "\"\"")}\"";
}
