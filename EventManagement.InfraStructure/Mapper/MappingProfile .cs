using AutoMapper;
using EventManagement.Core.DTOs;
using EventManagement.Core.Models;
using EventManagement.InfraStructure;
using EventManagement.Services;

namespace EventManagement.InfraStructure.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            
            CreateMap<Event, EventResponseDto>()
                .ForMember(dest => dest.SpeakerName, opt => opt.MapFrom(src => src.Speaker.Name))
                .ReverseMap();

            CreateMap<EventCreateDto, Event>().ReverseMap();
            CreateMap<EventUpdateDto, Event>().ReverseMap();

            
            CreateMap<Registration, RegistrationResponseDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
                .ForMember(dest => dest.EventTitle, opt => opt.MapFrom(src => src.Event.Title))
                .ReverseMap();

            CreateMap<RegistrationCreateDto, Registration>().ReverseMap();
            CreateMap<RegistrationUpdateDto, Registration>().ReverseMap();

            
            CreateMap<Speaker, SpeakerResponseDto>().ReverseMap();
            CreateMap<SpeakerCreateDto, Speaker>().ReverseMap();
            CreateMap<SpeakerUpdateDto, Speaker>().ReverseMap();

            
            CreateMap<User, UserResponseDto>().ReverseMap();
            CreateMap<UserCreateDto, User>().ReverseMap();
            CreateMap<UserUpdateDto, User>().ReverseMap();
        }
    }
}
