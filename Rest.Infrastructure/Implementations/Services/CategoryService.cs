using AutoMapper;
using Rest.Application.Dtos.CategoryDtos;
using Rest.Application.Interfaces.IServices;
using Rest.Domain.Entities;
using Rest.Domain.Interfaces.IRepositories;

namespace Rest.Infrastructure.Implementations.Services
{
    /// <summary>
    /// Service interface for managing categories.
    /// </summary>
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor for the CategoryService class.
        /// </summary>
        public CategoryService(ICategoryRepository repository, IMapper mapper)
        {
            _categoryRepository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Adds a new category to the database.
        /// </summary>
        /// <param name="category"> The category to add</param>
        /// <returns> The newly created category</returns>
        public async Task<Category> AddAsync(CategoryCreateDto category)
        {
            var categoryE = _mapper.Map<Category>(category);
            await _categoryRepository.AddAsync(categoryE);
            return categoryE;
        }

        /// <summary>
        /// Deactivates a category by setting its IsActive property to false.
        /// </summary>
        /// <param name="id"></param>
        public async Task DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category != null)
            {
                category.IsActive = false;

                foreach (var product in category.Products)
                {
                    product.IsAvailable = false;
                }
            }
            else
            {
                throw new KeyNotFoundException($"Category with ID {id} not found");
            }
        }

        /// <summary>
        /// Gets all categories from the database.
        /// </summary>
        /// <returns> List of all categories</returns>
        public async Task<IEnumerable<FullCategoryDto>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllWithProductsAsync();
            var categoriesDto = _mapper.Map<IEnumerable<FullCategoryDto>>(categories);
            return categoriesDto;
        }

        /// <summary>
        /// Gets all categories with their products
        /// </summary>
        /// <param name="id"> The ID of the category to retrieve</param>
        /// <returns> The category with the specified ID</returns>
        public async Task<FullCategoryDto> GetByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            var categoryDto = _mapper.Map<FullCategoryDto>(category);
            return categoryDto;
        }

        /// <summary>
        /// Saves changes to the database.
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _categoryRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="id"> The ID of the category to update</param>
        /// <param name="category"> The updated category data transfer object</param>
        /// <returns> Task representing the asynchronous operation</returns>
        public async Task Update(int id, CategoryUpdateDto category)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(id);
            if (existingCategory == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }
            _mapper.Map(category, existingCategory);
            _categoryRepository.Update(existingCategory);
        }
    }
}
