using AutoMapper;
using Rest.API.Dtos.AccountDtos;
using Rest.API.Models;

namespace Rest.API.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterDto, User>();
            CreateMap<LoginDto, User>();
        }
    }
}
