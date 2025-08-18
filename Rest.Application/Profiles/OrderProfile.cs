using AutoMapper;
using Rest.Application.Dtos.OrderDetailsDtos;
using Rest.Application.Dtos.OrderDtos;
using Rest.Domain.Entities;

namespace Rest.Application.Profiles
{
    public class OrderProfile :Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderDto>()
               .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.CustomerAddress, opt => opt.MapFrom(src => $"{src.DeliveryAddress.AddressLine1}, {src.DeliveryAddress.City}"))
                .ForMember(dest => dest.DeliveryPersonName, opt => opt.MapFrom(src => src.DeliveryPerson.FullName))
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.UserId))
                 .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));


            CreateMap<CreateOrderDto, Order>()
            .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails));

            CreateMap<CreateOrderDetailDto, OrderDetail>();

            CreateMap<UpdateOrderDto, Order>()
                .ForMember(dest => dest.OrderDetails, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<OrderDetail, OrderDetailDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : "Unknown"));
        }
    }
}
