using AutoMapper;
using EventManagement.Core.DTOs;
using EventManagement.Core.Models;
using EventManagement.Core.Repositories;
using FluentValidation;
using FluentValidation.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventManagement.Services.Services
{
    public class EventService
    {
        private readonly IEventManagementRepository _eventRepo;
        private readonly IValidator<Event> _validator;
        private readonly IMapper _mapper;

        public EventService(IEventManagementRepository eventRepo, IValidator<Event> validator, IMapper mapper)
        {
            _eventRepo = eventRepo;
            _validator = validator;
            _mapper = mapper;
        }

        
        public async Task<List<EventResponseDto>> GetAllAsync()
        {
            var events = await _eventRepo.GetAll();
            return _mapper.Map<List<EventResponseDto>>(events);
        }

        
        public async Task<EventResponseDto> GetByIdAsync(int id)
        {
            var ev = await _eventRepo.Get(id);
            return ev == null ? null : _mapper.Map<EventResponseDto>(ev);
        }

        
        public async Task<EventResponseDto> AddAsync(EventCreateDto dto)
        {
            var entity = _mapper.Map<Event>(dto);

            ValidationResult result = await _validator.ValidateAsync(entity);
            if (!result.IsValid)
                throw new ValidationException(result.Errors);

            var created = await _eventRepo.Create(entity);
            return _mapper.Map<EventResponseDto>(created);
        }

        
        public async Task<EventResponseDto> UpdateAsync(EventUpdateDto dto)
        {
            var entity = _mapper.Map<Event>(dto);

            ValidationResult result = await _validator.ValidateAsync(entity);
            if (!result.IsValid)
                throw new ValidationException(result.Errors);

            var updated = await _eventRepo.Update(entity);
            return _mapper.Map<EventResponseDto>(updated);
        }

        
        public async Task DeleteAsync(int id) => await _eventRepo.Delete(id);

        
        public async Task<List<EventResponseDto>> GetEventsByUser(int userId, bool upcomingOnly)
        {
            var events = await _eventRepo.GetEventsByUser(userId, upcomingOnly);
            return _mapper.Map<List<EventResponseDto>>(events);
        }

        
        public async Task<List<UserResponseDto>> GetUsersByEvent(int eventId)
        {
            var users = await _eventRepo.GetUsersByEvent(eventId);
            return _mapper.Map<List<UserResponseDto>>(users);
        }

        
        public async Task<List<EventResponseDto>> GetUpcomingEventsWithSpeakers()
        {
            var events = await _eventRepo.GetUpcomingEventsWithSpeakers();
            return _mapper.Map<List<EventResponseDto>>(events);
        }
    }
}
