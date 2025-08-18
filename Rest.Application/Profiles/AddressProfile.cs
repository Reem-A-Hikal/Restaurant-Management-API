using AutoMapper;
using Rest.Application.Dtos.AddressDtos;
using Rest.Domain.Entities;

namespace Rest.Application.Profiles
{
    public class AddressProfile : Profile
    {
        public AddressProfile()
        {

            CreateMap<AddressCreateDto, Address>();
            CreateMap<AddressUpdateDto, Address>();
            CreateMap<Address, AddressDto>().ReverseMap();
        }

    }
}
