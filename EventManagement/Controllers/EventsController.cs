using EventManagement.Core.ViewModels;
using EventManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Controllers;

[Authorize]
public class EventsController : Controller
{
    private readonly IEventDemoStore _store;

    public EventsController(IEventDemoStore store)
    {
        _store = store;
    }

    public IActionResult Index() => View(_store.GetEvents());

    public IActionResult Details(int id)
    {
        var model = _store.GetEvent(id);
        return model == null ? NotFound() : View(model);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View(new EventFormViewModel
    {
        StartDate = DateTime.Now.AddDays(1),
        EndDate = DateTime.Now.AddDays(1).AddHours(2),
        Category = "Conference",
        Status = "Active",
        MaxCapacity = 100
    });

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public IActionResult Create(EventFormViewModel model)
    {
        ValidateEvent(model);
        if (!ModelState.IsValid) return View(model);

        _store.CreateEvent(model, User.Identity?.Name ?? "Admin User", User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "admin@eventmanagement.local");
        TempData["Success"] = "Събитието беше създадено успешно.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Edit(int id)
    {
        var model = _store.GetEventForm(id);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public IActionResult Edit(EventFormViewModel model)
    {
        ValidateEvent(model);
        if (!ModelState.IsValid) return View(model);

        if (!_store.UpdateEvent(model)) return NotFound();
        TempData["Success"] = "Събитието беше обновено успешно.";
        return RedirectToAction(nameof(Details), new { id = model.Id });
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        var model = _store.GetEvent(id);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteConfirmed(int id)
    {
        if (!_store.DeleteEvent(id)) return NotFound();
        TempData["Success"] = "Събитието беше изтрито.";
        return RedirectToAction(nameof(Index));
    }

    private void ValidateEvent(EventFormViewModel model)
    {
        if (model.EndDate <= model.StartDate)
        {
            ModelState.AddModelError(nameof(model.EndDate), "Крайната дата трябва да е след началната.");
        }

        if (model.StartDate < DateTime.Now.AddMinutes(-1))
        {
            ModelState.AddModelError(nameof(model.StartDate), "Началната дата не може да е в миналото.");
        }
    }
}
