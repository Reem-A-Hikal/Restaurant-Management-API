using AutoMapper;
using Rest.Application.Dtos.ReviewDtos;
using Rest.Domain.Entities;

namespace Rest.Application.Profiles
{
    public class ReviewProfile : Profile
    {
        public ReviewProfile()
        {
            CreateMap<Review, ReviewDto>();

            CreateMap<CreateReviewDto, Review>()
                .ForMember(dest => dest.ReviewerName, opt => opt.Ignore())
                .ForMember(dest => dest.OrderId, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
                .ForMember(dest => dest.ReviewDate, opt => opt.Ignore());
        }
    }
}
