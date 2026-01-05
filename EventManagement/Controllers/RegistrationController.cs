using AutoMapper;
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
    public class RegistrationController : ControllerBase
    {
        private readonly RegistrationService _regService;

        public RegistrationController(RegistrationService regService)
        {
            _regService = regService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _regService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _regService.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RegistrationCreateDto model)
        {
            try
            {
                var created = await _regService.AddAsync(model);
                return CreatedAtAction(nameof(Get), new { id = created.RegistrationId }, created);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Errors);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, RegistrationUpdateDto model)
        {
            if (id != model.RegistrationId) return BadRequest("Registration ID mismatch");

            try
            {
                var updated = await _regService.UpdateAsync(model);
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
            await _regService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("by-event/{eventId}")]
        public async Task<IActionResult> GetByEvent(int eventId) =>
            Ok(await _regService.GetByEvent(eventId));

        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId) =>
            Ok(await _regService.GetByUser(userId));

        [HttpGet("by-status")]
        public async Task<IActionResult> GetByStatus(bool isRegistered) =>
            Ok(await _regService.GetByStatus(isRegistered));
    }
}
