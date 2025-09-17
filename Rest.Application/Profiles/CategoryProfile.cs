using AutoMapper;
using Rest.Application.Dtos.CategoryDtos;
using Rest.Application.Dtos.ProductDtos;
using Rest.Domain.Entities;

namespace Rest.Application.Profiles
{
    /// <summary>
    /// AutoMapper profile for mapping between Category and FullCategoryDto
    /// </summary>
    public class CategoryProfile : Profile
    {
        /// <summary>
        /// Constructor to create mappings
        /// </summary>
        public CategoryProfile()
        {
            CreateMap<Category, CategoryWithProductsDto>();

            CreateMap<CategoryCreateDto, Category>()
                .ForMember(dest => dest.Products, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<CategoryUpdateDto, Category>()
                .ForMember(dest => dest.Products, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Product, SimpleProductDto>();
        }
    }
}
