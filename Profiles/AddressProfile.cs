using AutoMapper;
using Rest.API.Dtos.AddressDtos;
using Rest.API.Models;

namespace Rest.API.Profiles
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
