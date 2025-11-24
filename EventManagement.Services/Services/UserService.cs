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
    public class UserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IValidator<User> _validator;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepo, IValidator<User> validator, IMapper mapper)
        {
            _userRepo = userRepo;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<List<UserResponseDto>> GetAllAsync()
        {
            var users = await _userRepo.GetAll();
            return _mapper.Map<List<UserResponseDto>>(users);
        }

        public async Task<UserResponseDto> GetByIdAsync(int id)
        {
            var user = await _userRepo.Get(id);
            return user == null ? null : _mapper.Map<UserResponseDto>(user);
        }

        public async Task<UserResponseDto> AddAsync(UserCreateDto dto)
        {
            var entity = _mapper.Map<User>(dto);
            ValidationResult result = await _validator.ValidateAsync(entity);
            if (!result.IsValid)
                throw new ValidationException(result.Errors);

            var created = await _userRepo.Create(entity);
            return _mapper.Map<UserResponseDto>(created);
        }

        public async Task<UserResponseDto> UpdateAsync(UserUpdateDto dto)
        {
            var entity = _mapper.Map<User>(dto);
            ValidationResult result = await _validator.ValidateAsync(entity);
            if (!result.IsValid)
                throw new ValidationException(result.Errors);

            var updated = await _userRepo.Update(entity);
            return _mapper.Map<UserResponseDto>(updated);
        }

        public async Task DeleteAsync(int id) => await _userRepo.Delete(id);

        public async Task<List<UserResponseDto>> GetUsersByEvent(int eventId)
        {
            var users = await _userRepo.GetUsersByEventId(eventId);
            return _mapper.Map<List<UserResponseDto>>(users);
        }

        public async Task<List<UserResponseDto>> GetUsersByRegistrationStatus(bool isRegistered)
        {
            var users = await _userRepo.GetUsersByRegistrationStatus(isRegistered);
            return _mapper.Map<List<UserResponseDto>>(users);
        }
    }
}
