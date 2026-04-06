using System.Security.Claims;
using EventManagement.Core.ViewModels;
using EventManagement.InfraStructure;
using EventManagement.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    private readonly EventDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AccountController(EventDbContext dbContext, IPasswordHasher<User> passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == model.Email.Trim().ToLower());
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Невалиден имейл или парола.");
            return View(model);
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "Невалиден имейл или парола.");
            return View(model);
        }

        await SignInUserAsync(user);
        TempData["Success"] = $"Добре дошъл, {user.FirstName}!";

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (model.DateOfBirth > DateTime.Today.AddYears(-14))
        {
            ModelState.AddModelError(nameof(model.DateOfBirth), "Потребителят трябва да е поне на 14 години.");
            return View(model);
        }

        var normalizedEmail = model.Email.Trim().ToLower();
        var emailExists = await _dbContext.Users.AnyAsync(u => u.Email == normalizedEmail);
        if (emailExists)
        {
            ModelState.AddModelError(nameof(model.Email), "Вече има потребител с този имейл.");
            return View(model);
        }

        var user = new User
        {
            FirstName = model.FirstName.Trim(),
            LastName = model.LastName.Trim(),
            Email = normalizedEmail,
            DateOfBirth = model.DateOfBirth,
            CreatedOn = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        await SignInUserAsync(user);
        TempData["Success"] = "Регистрацията е успешна. Можеш веднага да създадеш първото си събитие.";

        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["Success"] = "Излезе успешно от профила си.";
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return RedirectToAction(nameof(Login));
        }

        var user = await _dbContext.Users
            .Include(u => u.CreatedEvents)
                .ThenInclude(e => e.Registrations)
            .Include(u => u.Registrations)
            .FirstAsync(u => u.UserId == userId.Value);

        var model = new ProfileViewModel
        {
            FullName = user.DisplayName,
            Email = user.Email,
            DateOfBirth = user.DateOfBirth,
            MemberSince = user.CreatedOn,
            CreatedEventsCount = user.CreatedEvents.Count,
            RegisteredEventsCount = user.Registrations.Count,
            UpcomingOwnedEvents = user.CreatedEvents
                .Where(e => e.Date >= DateTime.Now)
                .OrderBy(e => e.Date)
                .Take(4)
                .Select(e => new EventViewModel
                {
                    Id = e.EventId,
                    Title = e.Title ?? string.Empty,
                    Category = e.Category,
                    Status = e.Status,
                    StartDate = e.Date,
                    EndDate = e.EndDate,
                    Location = e.Location,
                    MaxCapacity = e.MaxCapacity,
                    CurrentAttendees = e.Registrations.Count,
                    TicketPrice = e.TicketPrice,
                    CreatedOn = e.CreatedOn,
                    OrganizerName = user.DisplayName,
                    OrganizerEmail = user.Email,
                    IsOwner = true,
                    AvailableSpots = Math.Max(0, e.MaxCapacity - e.Registrations.Count)
                })
                .ToList()
        };

        return View(model);
    }

    private async Task SignInUserAsync(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name, user.DisplayName),
            new(ClaimTypes.Email, user.Email)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties { IsPersistent = true });
    }

    private int? GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out var userId) ? userId : null;
    }
}
