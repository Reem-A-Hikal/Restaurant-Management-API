using AutoMapper;
using Rest.API.Dtos.CategoryDtos;
using Rest.API.Dtos.ProductDtos;
using Rest.API.Models;

namespace Rest.API.Profiles
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
            CreateMap<Category, FullCategoryDto>();

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
