using AutoMapper;
using Rest.Application.Dtos.CategoryDtos;
using Rest.Application.Dtos.ProductDtos;
using Rest.Domain.Entities;

namespace Rest.Application.Profiles
{
    /// <summary>
    /// AutoMapper profile for mapping between Product and ProductDto
    /// </summary>
    public class ProductProfile :Profile
    {
        /// <summary>
        /// Constructor to create mappings
        /// </summary>
        public ProductProfile()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.DiscountedPrice,
                    opt => opt.MapFrom(src => src.GetDiscountedPrice()));

            CreateMap<Product, SimpleProductDto>();
        }
    }
}
