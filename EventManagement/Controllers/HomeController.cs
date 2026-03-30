using EventManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IEventDemoStore _store;

    public HomeController(IEventDemoStore store)
    {
        _store = store;
    }

    public IActionResult Index() => View(_store.GetDashboard());
}
