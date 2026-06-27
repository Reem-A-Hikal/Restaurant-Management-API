using AutoMapper;
using Rest.Application.Dtos.OrderDetailsDtos;
using Rest.Application.Dtos.OrderDtos;
using Rest.Domain.Entities;
using Rest.Domain.Entities.Enums;

namespace Rest.Application.Profiles
{
    public class OrderProfile :Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderDto>()
               .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.User.FullName))
               .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.UserId))
               .ForMember(dest => dest.CustomerAddress, opt => opt.MapFrom(src => $"{src.DeliveryAddress.AddressLine1}, {src.DeliveryAddress.City}"))
               .ForMember(dest => dest.StatusDisplay, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CreateOrderDto, Order>()
            .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails));

            CreateMap<CreateOrderDetailDto, OrderDetail>();

            CreateMap<OrderDetail, OrderDetailDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : "Unknown"));
        }
    }
}
