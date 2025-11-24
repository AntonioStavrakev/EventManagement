using EventManagement.Core.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement.Services.Validators
{
    public class EventValidator : AbstractValidator<Event>
    {
        public EventValidator()
        {
            //RuleFor(e => e.Title).NotEmpty().WithMessage("Title is required");
            RuleFor(e => e.Date)
                .Must(date => date > DateTime.Now)
                .WithMessage("Event date must be in the future");
        }
    }
}
