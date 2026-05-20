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
            CreateMap<Category, CategoryWithProductsDto>()
                .ForMember(dest => dest.TotalItems,
                    opt => opt.MapFrom(src => src.Products != null ? src.Products.Count : 0));

            CreateMap<CategoryCreateDto, Category>()
                .ForMember(dest => dest.Products, opt => opt.Ignore());


            CreateMap<CategoryUpdateDto, Category>()
                .ForMember(dest => dest.Products, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition(
                    (src, dest, srcMember) => srcMember != null));

            CreateMap<Category, CategoryUpdateDto>();

            CreateMap<Product, SimpleProductDto>();
        }
    }
}
