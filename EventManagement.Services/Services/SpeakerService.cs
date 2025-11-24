using AutoMapper;
using EventManagement.Core.DTOs;
using EventManagement.Core.Repositories;
using EventManagement.InfraStructure;
using FluentValidation;
using FluentValidation.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventManagement.Services.Services
{
    public class SpeakerService
    {
        private readonly ISpeakerRepository _speakerRepo;
        private readonly IValidator<Speaker> _validator;
        private readonly IMapper _mapper;

        public SpeakerService(ISpeakerRepository speakerRepo, IValidator<Speaker> validator, IMapper mapper)
        {
            _speakerRepo = speakerRepo;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<List<SpeakerResponseDto>> GetAllAsync()
        {
            var speakers = await _speakerRepo.GetAll();
            return _mapper.Map<List<SpeakerResponseDto>>(speakers);
        }

        public async Task<SpeakerResponseDto> GetByIdAsync(int id)
        {
            var speaker = await _speakerRepo.Get(id);
            return speaker == null ? null : _mapper.Map<SpeakerResponseDto>(speaker);
        }

        public async Task<SpeakerResponseDto> AddAsync(SpeakerCreateDto dto)
        {
            var entity = _mapper.Map<Speaker>(dto);
            ValidationResult result = await _validator.ValidateAsync(entity);
            if (!result.IsValid)
                throw new ValidationException(result.Errors);

            var created = await _speakerRepo.Create(entity);
            return _mapper.Map<SpeakerResponseDto>(created);
        }

        public async Task<SpeakerResponseDto> UpdateAsync(SpeakerUpdateDto dto)
        {
            var entity = _mapper.Map<Speaker>(dto);
            ValidationResult result = await _validator.ValidateAsync(entity);
            if (!result.IsValid)
                throw new ValidationException(result.Errors);

            var updated = await _speakerRepo.Update(entity);
            return _mapper.Map<SpeakerResponseDto>(updated);
        }

        public async Task DeleteAsync(int id) => await _speakerRepo.Delete(id);

        public async Task<List<SpeakerResponseDto>> GetSpeakersByEvent(int eventId)
        {
            var speakers = await _speakerRepo.GetSpeakersByEventId(eventId);
            return _mapper.Map<List<SpeakerResponseDto>>(speakers);
        }
    }
}
