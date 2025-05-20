using AutoMapper;
using Rest.API.Dtos.CategoryDtos;
using Rest.API.Models;
using Rest.API.Repositories.Interfaces;
using Rest.API.Services.Interfaces;

namespace Rest.API.Services.Implementations
{
    /// <summary>
    /// Service interface for managing categories.
    /// </summary>
    public class CategoryService : ICategoryService
    {
        ICategoryRepository _categoryRepository;
        IMapper _mapper;

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
            await _categoryRepository.SaveChangesAsync();
            return categoryE;
        }

        /// <summary>
        /// Deletes a category by its ID.
        /// </summary>
        /// <param name="id"> The ID of the category to delete</param>
        public async Task DeleteAsync(int id)
        {
            await _categoryRepository.DeleteAsync(id);
            await _categoryRepository.SaveChangesAsync();
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
            await _categoryRepository.SaveChangesAsync();
        }
    }
}
