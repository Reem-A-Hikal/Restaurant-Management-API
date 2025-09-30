using AutoMapper;
using Rest.Application.Dtos.CategoryDtos;
using Rest.Application.Interfaces.IRepositories;
using Rest.Application.Interfaces.IServices;
using Rest.Application.Utilities;
using Rest.Domain.Entities;

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
        public async Task<CategoryCreateDto> AddAsync(CategoryCreateDto category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category), "Category cannot be null");
            }
            var categoryE = _mapper.Map<Category>(category);
            await _categoryRepository.AddAsync(categoryE);
            await _categoryRepository.SaveChangesAsync();
            return category;
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
        public async Task<IEnumerable<CategoryWithProductsDto>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllWithProductsAsync();
            var categoriesDto = _mapper.Map<IEnumerable<CategoryWithProductsDto>>(categories);
            return categoriesDto;
        }

        public async Task<PaginatedList<CategoryUpdateDto>> GetPaginatedCatsWithFilterAsync(int pageIndex, int pageSize, string? searchTerm, string? selectedFilter)
        {
            try
            {
                var query = _categoryRepository.GetFilteredCats(searchTerm, selectedFilter);

                var paginatedCategories = await PaginatedList<Category>.CreateAsync(query, pageIndex, pageSize);
                var categoriesDto = _mapper.Map<List<CategoryUpdateDto>>(paginatedCategories.Items);
                return new PaginatedList<CategoryUpdateDto>(categoriesDto, paginatedCategories.TotalItems, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Could not retrieve paginated Users records.", ex);
            }
        }

        /// <summary>
        /// Gets all categories with their products
        /// </summary>
        /// <param name="id"> The ID of the category to retrieve</param>
        /// <returns> The category with the specified ID</returns>
        public async Task<CategoryWithProductsDto> GetByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Category with ID {id} not found.");
            var categoryDto = _mapper.Map<CategoryWithProductsDto>(category);
            return categoryDto;
        }

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="id"> The ID of the category to update</param>
        /// <param name="category"> The updated category data transfer object</param>
        /// <returns> Task representing the asynchronous operation</returns>
        public async Task Update(int id, CategoryUpdateDto category)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Category with ID {id} not found.");

            existingCategory.Name = category.Name ?? existingCategory.Name;
            existingCategory.Description = category.Description ?? existingCategory.Description;
            existingCategory.IsActive = category.IsActive ?? existingCategory.IsActive;
            existingCategory.CategoryId = id;

            _categoryRepository.Update(existingCategory);
            await _categoryRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Saves changes to the database.
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _categoryRepository.SaveChangesAsync();
        }
    }
}
