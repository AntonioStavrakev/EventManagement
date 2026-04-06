using System.ComponentModel.DataAnnotations;

namespace EventManagement.Core.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Имейлът е задължителен.")]
    [EmailAddress(ErrorMessage = "Въведи валиден имейл.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Паролата е задължителна.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}

public class RegisterViewModel
{
    [Required(ErrorMessage = "Името е задължително.")]
    [StringLength(60)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Фамилията е задължителна.")]
    [StringLength(60)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Имейлът е задължителен.")]
    [EmailAddress(ErrorMessage = "Въведи валиден имейл.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Рождената дата е задължителна.")]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; } = DateTime.Today.AddYears(-18);

    [Required(ErrorMessage = "Паролата е задължителна.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Паролата трябва да е поне 6 символа.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Потвърждението е задължително.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Паролите не съвпадат.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class ProfileViewModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public DateTime MemberSince { get; set; }
    public int CreatedEventsCount { get; set; }
    public int RegisteredEventsCount { get; set; }
    public IEnumerable<EventViewModel> UpcomingOwnedEvents { get; set; } = Array.Empty<EventViewModel>();
}
