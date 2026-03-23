using EventManagement.InfraStructure;
using NUnit.Framework;
namespace EventManagement.Tests.ValidatorTests;

[TestFixture]
public class RegistrationValidatorTests
{
    [Test]
    public void RegistrationDate_InPast_ShouldFail()
    {
        var v = new RegistrationValidator();

        var r = new Registration
        {
            UserId = 1,
            EventId = 1,
            RegistrationDate = DateTime.Now.AddDays(-1)
        };

        var result = v.Validate(r);

        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void UserId_Or_EventId_Zero_ShouldFail()
    {
        var v = new RegistrationValidator();

        var r = new Registration
        {
            UserId = 0,
            EventId = 0,
            RegistrationDate = DateTime.Now
        };

        var result = v.Validate(r);

        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void Valid_Registration_ShouldPass()
    {
        var v = new RegistrationValidator();

        var r = new Registration
        {
            UserId = 1,
            EventId = 2,
            RegistrationDate = DateTime.Now
        };

        var result = v.Validate(r);

        Assert.That(result.IsValid, Is.True);
    }
}