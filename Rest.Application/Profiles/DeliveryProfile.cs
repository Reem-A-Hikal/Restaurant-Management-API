using AutoMapper;
using Rest.Application.Dtos.DeliveryDtos;
using Rest.Domain.Entities;

namespace Rest.Application.Profiles
{
    public class DeliveryProfile : Profile
    {
        public DeliveryProfile()
        {
            CreateMap<DeliveryDto, DeliveryDto>();
        }
    }
}
