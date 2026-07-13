using AutoMapper;
using Rest.Application.Dtos.DeliveryDtos;
using Rest.Domain.Entities;

namespace Rest.Application.Profiles
{
    public class DeliveryProfile : Profile
    {
        public DeliveryProfile()
        {
            CreateMap<Delivery, DeliveryDto>()
                .ForMember(dest => dest.OrderNumber, 
                    opt => opt.MapFrom(src => src.Order != null ? src.Order.OrderNumber : string.Empty))
                .ForMember(dest => dest.DeliveryPersonName,
                    opt => opt.MapFrom(src => src.DeliveryPerson != null ? src.DeliveryPerson.FullName : string.Empty))
                .ForMember(dest => dest.CustomerAddress,
                    opt => opt.MapFrom(src => src.Order != null && src.Order.DeliveryAddress != null
                        ? $"{src.Order.DeliveryAddress.AddressLine1}, {src.Order.DeliveryAddress.City}"
                        : string.Empty));

            CreateMap<DeliveryPerson, AvailableDeliveryPersonDto>();
        }
    }
}
