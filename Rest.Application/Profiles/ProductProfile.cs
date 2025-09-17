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
            CreateMap<Product, ProductDto>();

            CreateMap<ProductCreateDto, Product>()
                .ForMember(dest => dest.OrderDetails, opt => opt.Ignore())
                .ForMember(dest => dest.Reviews, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<ProductUpdateDto, Product>()
                .ForMember(dest => dest.OrderDetails, opt => opt.Ignore())
                .ForMember(dest => dest.Reviews, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Category, SimpleCategoryDto>();
        }
    }
}
