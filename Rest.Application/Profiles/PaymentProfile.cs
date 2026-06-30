using AutoMapper;
using Rest.Application.Dtos.PaymentDtos;
using Rest.Domain.Entities;

namespace Rest.Application.Profiles
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<Payment, PaymentDto>();
        }
    }
}
