using EventManagement.Core.DTOs;
using EventManagement.Services.Services;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace EventManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly EventService _eventService;

        public EventController(EventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _eventService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _eventService.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EventCreateDto model)
        {
            try
            {
                var created = await _eventService.AddAsync(model);
                return CreatedAtAction(nameof(Get), new { id = created.EventId }, created);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Errors);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, EventUpdateDto model)
        {
            if (id != model.EventId) return BadRequest("Event ID mismatch");

            try
            {
                var updated = await _eventService.UpdateAsync(model);
                return Ok(updated);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Errors);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _eventService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetEventsByUser(int userId, bool upcomingOnly = true) =>
            Ok(await _eventService.GetEventsByUser(userId, upcomingOnly));

        [HttpGet("{eventId}/users")]
        public async Task<IActionResult> GetUsersByEvent(int eventId) =>
            Ok(await _eventService.GetUsersByEvent(eventId));

        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcomingEventsWithSpeakers() =>
            Ok(await _eventService.GetUpcomingEventsWithSpeakers());
    }
}
