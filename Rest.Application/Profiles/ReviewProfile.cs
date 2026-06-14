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
        }
    }
}
