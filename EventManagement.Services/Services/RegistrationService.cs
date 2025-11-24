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
    public class RegistrationService
    {
        private readonly IRegistrationReposiory _regRepo;
        private readonly IValidator<Registration> _validator;
        private readonly IMapper _mapper;

        public RegistrationService(IRegistrationReposiory regRepo, IValidator<Registration> validator, IMapper mapper)
        {
            _regRepo = regRepo;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<List<RegistrationResponseDto>> GetAllAsync()
        {
            var regs = await _regRepo.GetAll();
            return _mapper.Map<List<RegistrationResponseDto>>(regs);
        }

        public async Task<RegistrationResponseDto> GetByIdAsync(int id)
        {
            var reg = await _regRepo.Get(id);
            return reg == null ? null : _mapper.Map<RegistrationResponseDto>(reg);
        }

        public async Task<RegistrationResponseDto> AddAsync(RegistrationCreateDto dto)
        {
            var entity = _mapper.Map<Registration>(dto);
            ValidationResult result = await _validator.ValidateAsync(entity);
            if (!result.IsValid)
                throw new ValidationException(result.Errors);

            var created = await _regRepo.Create(entity);
            return _mapper.Map<RegistrationResponseDto>(created);
        }

        public async Task<RegistrationResponseDto> UpdateAsync(RegistrationUpdateDto dto)
        {
            var entity = _mapper.Map<Registration>(dto);
            ValidationResult result = await _validator.ValidateAsync(entity);
            if (!result.IsValid)
                throw new ValidationException(result.Errors);

            var updated = await _regRepo.Update(entity);
            return _mapper.Map<RegistrationResponseDto>(updated);
        }

        public async Task DeleteAsync(int id) => await _regRepo.Delete(id);

        public async Task<List<RegistrationResponseDto>> GetByEvent(int eventId)
        {
            var regs = await _regRepo.GetRegistrationsByEventId(eventId);
            return _mapper.Map<List<RegistrationResponseDto>>(regs);
        }

        public async Task<List<RegistrationResponseDto>> GetByUser(int userId)
        {
            var regs = await _regRepo.GetRegistrationsByUserId(userId);
            return _mapper.Map<List<RegistrationResponseDto>>(regs);
        }

        public async Task<List<RegistrationResponseDto>> GetByStatus(bool isRegistered)
        {
            var regs = await _regRepo.GetRegistrationsByStatus(isRegistered);
            return _mapper.Map<List<RegistrationResponseDto>>(regs);
        }
    }
}
