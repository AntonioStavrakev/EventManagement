using EventManagement.Core.DTOs;
using EventManagement.Services.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpeakerController : ControllerBase
    {
        private readonly SpeakerService _speakerService;

        public SpeakerController(SpeakerService speakerService)
        {
            _speakerService = speakerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _speakerService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _speakerService.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SpeakerCreateDto model)
        {
            try
            {
                var created = await _speakerService.AddAsync(model);
                return CreatedAtAction(nameof(Get), new { id = created.SpeakerId }, created);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Errors);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SpeakerUpdateDto model)
        {
            if (id != model.SpeakerId) return BadRequest("Speaker ID mismatch");

            try
            {
                var updated = await _speakerService.UpdateAsync(model);
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
            await _speakerService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("by-event/{eventId}")]
        public async Task<IActionResult> GetSpeakersByEvent(int eventId) =>
            Ok(await _speakerService.GetSpeakersByEvent(eventId));
    }
}
