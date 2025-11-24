using EventManagement.InfraStructure;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement.Services.Validators
{
    public class SpeakerValidator : AbstractValidator<Speaker>
    {
        public SpeakerValidator()
        {
            RuleFor(s => s.Name).NotEmpty();
            RuleFor(s => s.Biography).NotEmpty();
            RuleFor(s => s.Email).NotEmpty().EmailAddress().WithMessage("Invalid email format");
        }
    }
}
