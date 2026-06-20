using AutoMapper;
using Rest.Application.Dtos.CategoryDtos;
using Rest.Application.Interfaces.IRepositories;
using Rest.Application.Interfaces.IServices;
using Rest.Application.Utilities;
using Rest.Domain.Entities;
using Rest.Domain.Entities.Enums;
using Rest.Domain.Exceptions;

namespace Rest.Application.Services
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
        public async Task<CategoryWithProductsDto> AddAsync(CategoryCreateDto dto)
        {
            if (dto == null)
                throw new ValidationException("Category data is required");

            var category = _mapper.Map<Category>(dto);
            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

            return _mapper.Map<CategoryWithProductsDto>(category);
        }

        /// <summary>
        /// Deactivates a category by setting its IsAvailable property to false.
        /// </summary>
        /// <param name="id"></param>
        public async Task ArchiveAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id)
                 ?? throw new NotFoundException("Category", id);

            if (category.Status == CategoryStatus.Archived)
                throw new BusinessException("Category is already archived");

            category.Status = CategoryStatus.Archived;

            foreach (var product in category.Products)
                product.Status = ProductStatus.Unavailable;

            await _categoryRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Gets all categories from the database.
        /// </summary>
        /// <returns> List of all categories</returns>
        public async Task<IEnumerable<CategoryWithProductsDto>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllWithProductsAsync();
            return _mapper.Map<IEnumerable<CategoryWithProductsDto>>(categories);
        }

        public async Task<PaginatedList<CategoryWithProductsDto>> GetPaginatedCatsWithFilterAsync(int pageIndex, int pageSize, string? searchTerm, string? selectedStatus)
        {
            var paginated = await _categoryRepository.GetPaginatedAsync(
                pageIndex, pageSize, searchTerm, selectedStatus);

            var dtos = _mapper.Map<List<CategoryWithProductsDto>>(paginated.Items);
            return new PaginatedList<CategoryWithProductsDto>(
                dtos, paginated.TotalItems, pageIndex, pageSize);
        }

        /// <summary>
        /// Gets all categories with their products
        /// </summary>
        /// <param name="id"> The ID of the category to retrieve</param>
        /// <returns> The category with the specified ID</returns>
        public async Task<CategoryWithProductsDto> GetByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id)
                ?? throw new NotFoundException("Category", id);

            return _mapper.Map<CategoryWithProductsDto>(category);
        }

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="id"> The ID of the category to update</param>
        /// <param name="dto"> The updated category data transfer object</param>
        /// <returns> Task representing the asynchronous operation</returns>
        public async Task UpdateAsync(int id, CategoryUpdateDto dto)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(id)
                 ?? throw new NotFoundException("Category", id);

            if (!string.IsNullOrWhiteSpace(dto.Name))
                existingCategory.Name = dto.Name;

            if (dto.Status.HasValue)
                existingCategory.Status = dto.Status.Value;

            if (dto.DisplayOrder.HasValue)
                existingCategory.DisplayOrder = dto.DisplayOrder.Value;

            _categoryRepository.Update(existingCategory);
            await _categoryRepository.SaveChangesAsync();
        }
    }
}
