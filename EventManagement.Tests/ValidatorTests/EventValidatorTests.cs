using EventManagement.Core.Models;
using EventManagement.Services.Validators;
using NUnit.Framework;
namespace EventManagement.Tests.ValidatorTests;

[TestFixture]
public class EventValidatorTests
{
    [Test]
    public void EventDate_InPast_ShouldFail()
    {
        var v = new EventValidator();
        var ev = new Event { Date = DateTime.Now.AddMinutes(-5) };

        var result = v.Validate(ev);

        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void EventDate_InFuture_ShouldPass()
    {
        var v = new EventValidator();
        var ev = new Event { Date = DateTime.Now.AddDays(1) };

        var result = v.Validate(ev);

        Assert.That(result.IsValid, Is.True);
    }
}