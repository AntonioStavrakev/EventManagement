using System.ComponentModel.DataAnnotations;

namespace EventManagement.Core.ViewModels;

public class EventFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Заглавието е задължително.")]
    [StringLength(120, ErrorMessage = "Заглавието трябва да е до 120 символа.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1200, ErrorMessage = "Описанието трябва да е до 1200 символа.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Избери категория.")]
    public string Category { get; set; } = "Conference";

    [Required]
    public string Status { get; set; } = "Active";

    [Required(ErrorMessage = "Началната дата е задължителна.")]
    [DataType(DataType.DateTime)]
    public DateTime StartDate { get; set; } = DateTime.Now.AddDays(1).Date.AddHours(18);

    [Required(ErrorMessage = "Крайната дата е задължителна.")]
    [DataType(DataType.DateTime)]
    public DateTime EndDate { get; set; } = DateTime.Now.AddDays(1).Date.AddHours(20);

    [Required(ErrorMessage = "Локацията е задължителна.")]
    [StringLength(160, ErrorMessage = "Локацията трябва да е до 160 символа.")]
    public string Location { get; set; } = string.Empty;

    [Range(1, 50000, ErrorMessage = "Капацитетът трябва да е между 1 и 50000.")]
    public int MaxCapacity { get; set; } = 50;

    [Range(0, 100000, ErrorMessage = "Цената трябва да е положителна.")]
    public decimal TicketPrice { get; set; }
}
