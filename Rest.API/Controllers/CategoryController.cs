using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Rest.Application.Dtos.CategoryDtos;
using Rest.Application.Interfaces.IServices;

namespace Rest.API.Controllers
{
    /// <summary>
    /// Controller for managing categories.
    /// </summary>
    [Route("api/category")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CategoryController : BaseController
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        /// <summary>
        /// Constructor for CategoryController.
        /// </summary>
        /// <param name="categoryService"></param>
        /// <param name="logger"></param>
        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        /// <summary>
        /// get all categories
        /// </summary>
        /// <returns></returns>

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var categories = await _categoryService.GetAllAsync();
                return SuccessResponse(categories, "Categories retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all categories");
                return InternalErrorResponse(ex);
            }
        }
        /// <summary>
        /// get all categories with pagination and filter
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchTerm"></param>
        /// <param name="selectedFilter"></param>
        /// <returns></returns>

        [HttpGet("GetAllPaginated")]
        public async Task<IActionResult> GetPaginated(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = "",
            [FromQuery] string? selectedFilter = "All")
        {
            try
            {
                var categories = await _categoryService.GetPaginatedCatsWithFilterAsync(
                    pageIndex, pageSize, searchTerm, selectedFilter);
                return SuccessResponse(categories, "Categories retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting paginated categories");
                return InternalErrorResponse(ex);
            }
        }
        /// <summary>
        /// get category by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("getcategory/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var category = await _categoryService.GetByIdAsync(id);
                return SuccessResponse(category, "Category retrieved successfully");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Category not found");
                return ValidationErrorResponse(["Category not found."]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting category with ID {id}", id);
                return InternalErrorResponse(ex);
            }
        }

        /// <summary>
        /// Add a new category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] CategoryCreateDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationErrorResponse(
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));

            try
            {
                var createdCategory = await _categoryService.AddAsync(dto);
                return CreatedResponse(
                    nameof(GetById),
                    new { id = createdCategory.CategoryId },
                    createdCategory,
                    "Category created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a new category");
                return InternalErrorResponse(ex);
            }
        }
        /// <summary>
        /// Update a category by ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateDto request)
        {
            if (!ModelState.IsValid)
                return ValidationErrorResponse(
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));
            try
            {
                await _categoryService.UpdateAsync(id, request);
                return SuccessResponse<string>(null, "Category updated successfully");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Category not found with ID {id}", id);
                return NotFoundResponse(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating category with ID {id}", id);
                return InternalErrorResponse(ex);
            }
        }
        /// <summary>
        /// Delete a category by ID (soft delete / deactivate).
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Archive(int id)
        {
            try
            {
                await _categoryService.ArchiveAsync(id);
                return SuccessResponse<string>(null, "Category archived successfully");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Category not found with ID {id}", id);
                return NotFoundResponse(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error archiving category: {Id}", id);
                return InternalErrorResponse(ex);
            }
        }
    }
}
