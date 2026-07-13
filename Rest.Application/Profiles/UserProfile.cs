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
            CreateMap<User, UserDto>();
        }
    }
}
