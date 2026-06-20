using AutoMapper;
using Rest.Application.Dtos.AccountDtos;
using Rest.Application.Dtos.UserDtos;
using Rest.Domain.Entities;
using Rest.Domain.Entities.Enums;

namespace Rest.Application.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => UserStatus.Active))
                .ForMember(dest => dest.JoinDate, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<User, UserDto>();

            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => UserStatus.Active))
                .ForMember(dest => dest.JoinDate, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<CreateUserDto, Chef>()
               .IncludeBase<CreateUserDto, User>() 
                .ForMember(dest => dest.Specialization, opt => opt.MapFrom(src => src.Specialization));

            CreateMap<CreateUserDto, DeliveryPerson>()
                .IncludeBase<CreateUserDto, User>()
                .ForMember(dest => dest.VehicleNumber, opt => opt.MapFrom(src => src.VehicleNumber))
                .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.IsAvailable));

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
                });
        }
    }
}
