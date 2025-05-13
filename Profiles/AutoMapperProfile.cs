using AutoMapper;
using Rest.API.Dtos.AccountDtos;
using Rest.API.Models;

namespace Rest.API.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.JoinDate, opt => opt.MapFrom(src => DateTime.UtcNow));



            CreateMap<LoginDto, User>();
        }
    }
}
