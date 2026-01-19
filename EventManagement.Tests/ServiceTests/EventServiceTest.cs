using AutoMapper;
using EventManagement.Core.DTOs;
using EventManagement.Core.Models;
using EventManagement.Core.Repositories;
using EventManagement.InfraStructure.Mapper;
using EventManagement.Services.Services;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using NUnit.Framework;
namespace EventManagement.Tests.ServiceTests;


[TestFixture]
public class EventServiceTest
{
   private static IMapper CreateMapper()
    {
        var cfg = new MapperConfiguration(c => c.AddProfile<MappingProfile>());
        return cfg.CreateMapper();
    }

    [Test]
    public async Task GetByIdAsync_WhenRepoReturnsNull_ReturnsNull()
    {
        var repo = new Mock<IEventManagementRepository>();
        repo.Setup(r => r.Get(It.IsAny<int>())).ReturnsAsync((Event?)null);

        var validator = new Mock<IValidator<Event>>();
        var mapper = CreateMapper();

        var sut = new EventService(repo.Object, validator.Object, mapper);

        var result = await sut.GetByIdAsync(123);

        Assert.That(result, Is.Null);
        repo.Verify(r => r.Get(123), Times.Once);
    }

    [Test]
    public void AddAsync_WhenValidationFails_ThrowsValidationException_AndDoesNotCreate()
    {
        var repo = new Mock<IEventManagementRepository>();

        var validator = new Mock<IValidator<Event>>();
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<Event>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[]
            {
                new ValidationFailure("Date", "Event date must be in the future")
            }));

        var mapper = CreateMapper();
        var sut = new EventService(repo.Object, validator.Object, mapper);

        var dto = new EventCreateDto
        {
            Title = "Test",
            Description = "Desc",
            Location = "Sofia",
            SpeakerId = 1,
            Date = DateTime.Now.AddDays(-1)
        };

        Assert.ThrowsAsync<ValidationException>(async () => await sut.AddAsync(dto));
        repo.Verify(r => r.Create(It.IsAny<Event>()), Times.Never);
    }

    [Test]
    public async Task AddAsync_WhenValid_CallsCreateAndReturnsMappedResponse()
    {
        var repo = new Mock<IEventManagementRepository>();

        repo.Setup(r => r.Create(It.IsAny<Event>()))
            .ReturnsAsync((Event e) =>
            {
                e.EventId = 10;
                // MappingProfile очаква Speaker.Name да не е null
                e.Speaker = new EventManagement.InfraStructure.Speaker { Name = "John Speaker" };
                return e;
            });

        var validator = new Mock<IValidator<Event>>();
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<Event>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult()); // valid

        var mapper = CreateMapper();
        var sut = new EventService(repo.Object, validator.Object, mapper);

        var dto = new EventCreateDto
        {
            Title = "My Event",
            Description = "Desc",
            Location = "Sofia",
            SpeakerId = 2,
            Date = DateTime.Now.AddDays(5)
        };

        var result = await sut.AddAsync(dto);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.EventId, Is.EqualTo(10));
        Assert.That(result.Title, Is.EqualTo("My Event"));
        Assert.That(result.SpeakerId, Is.EqualTo(2));
        Assert.That(result.SpeakerName, Is.EqualTo("John Speaker"));

        repo.Verify(r => r.Create(It.IsAny<Event>()), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_CallsRepoDelete()
    {
        var repo = new Mock<IEventManagementRepository>();
        var validator = new Mock<IValidator<Event>>();
        var mapper = CreateMapper();

        var sut = new EventService(repo.Object, validator.Object, mapper);

        await sut.DeleteAsync(7);

        repo.Verify(r => r.Delete(7), Times.Once);
    }
}