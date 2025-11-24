using EventManagement.InfraStructure;
using FluentValidation;

public class RegistrationValidator : AbstractValidator<Registration>
{
    public RegistrationValidator()
    {
        RuleFor(r => r.RegistrationDate)
            .GreaterThanOrEqualTo(DateTime.Now)
            .WithMessage("Registration date cannot be in the past.");

        RuleFor(r => r.UserId).GreaterThan(0);
        RuleFor(r => r.EventId).GreaterThan(0);
    }
}
