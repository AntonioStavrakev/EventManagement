using EventManagement.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        var model = new DashboardViewModel
        {
            TotalEvents = 0,
            ActiveEvents = 0,
            TotalAttendees = 0,
            UpcomingEvents = 0,
            RecentEvents = new List<EventViewModel>()
        };

        return View(model);
    }
}