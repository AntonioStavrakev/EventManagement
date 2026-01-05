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
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _userService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _userService.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserCreateDto model)
        {
            try
            {
                var created = await _userService.AddAsync(model);
                return CreatedAtAction(nameof(Get), new { id = created.UserId }, created);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Errors);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UserUpdateDto model)
        {
            if (id != model.UserId) return BadRequest("User ID mismatch");

            try
            {
                var updated = await _userService.UpdateAsync(model);
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
            await _userService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("by-event/{eventId}")]
        public async Task<IActionResult> GetUsersByEvent(int eventId) =>
            Ok(await _userService.GetUsersByEvent(eventId));

        [HttpGet("by-status")]
        public async Task<IActionResult> GetUsersByStatus(bool isRegistered) =>
            Ok(await _userService.GetUsersByRegistrationStatus(isRegistered));
    }
}
