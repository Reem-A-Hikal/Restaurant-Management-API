using AutoMapper;
using Rest.Application.Dtos.AccountDtos;
using Rest.Application.Dtos.UserDtos;
using Rest.Domain.Entities;

namespace Rest.Application.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
                .ForMember(dest => dest.JoinDate, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Roles, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    dest.SetSpecialization(src.Specialization);
                    dest.SetVehicleNumber(src.VehicleNumber);
                });

            CreateMap<LoginDto, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            CreateMap<UpdateProfileDto, User>()
                .ForMember(dest => dest.FullName, opt =>
                {
                    opt.MapFrom(src => src.FullName);
                    opt.Condition(src => !string.IsNullOrEmpty(src.FullName));
                })
                .ForMember(dest => dest.PhoneNumber, opt =>
                {
                    opt.MapFrom(src => src.PhoneNumber);
                    opt.Condition(src => !string.IsNullOrEmpty(src.PhoneNumber));
                })
                .ForMember(dest => dest.ProfileImageUrl, opt =>
                {
                    opt.MapFrom(src => src.ProfileImageUrl);
                    opt.Condition(src => !string.IsNullOrEmpty(src.ProfileImageUrl));
                })
                .ForMember(dest => dest.Specialization, opt =>
                {
                    opt.MapFrom(src => src.Specialization);
                    opt.Condition(src => !string.IsNullOrEmpty(src.Specialization));
                })
                .ForMember(dest => dest.VehicleNumber, opt =>
                {
                    opt.MapFrom(src => src.VehicleNumber);
                    opt.Condition(src => !string.IsNullOrEmpty(src.VehicleNumber));
                });

            
        }
    }
}
