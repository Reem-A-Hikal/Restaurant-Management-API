using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rest.Application.Dtos.CategoryDtos;
using Rest.Application.IServices;

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
        private readonly ILogger<CategoryController> logger;

        /// <summary>
        /// Constructor for CategoryController.
        /// </summary>
        /// <param name="categoryService"></param>
        /// <param name="logger"></param>
        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            this.logger = logger;
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
                logger.LogError(ex, "Error occurred while getting all categories");
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
        public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = "", [FromQuery] string? selectedFilter = "")
        {
            try
            {
                var paginatedCategories = await _categoryService.GetPaginatedCatsWithFilterAsync(pageIndex, pageSize, searchTerm, selectedFilter);
                return SuccessResponse(paginatedCategories, "Categories retrieved successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while getting paginated categories");
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
                logger.LogWarning(ex, "Category not found");
                return ValidationErrorResponse(["Category not found."]);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while getting category with ID {id}", id);
                return InternalErrorResponse(ex);
            }
        }

        /// <summary>
        /// Add a new category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] CategoryCreateDto category)
        {
            if (category == null)
            {
                return ValidationErrorResponse(["Category data is required."]);
            }
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                return ValidationErrorResponse(errors);
            }
            try
            {
                var createdCategory = await _categoryService.AddAsync(category);
                return SuccessResponse(createdCategory, "Category created successfully");
            }
            catch (ArgumentNullException ex)
            {
                logger.LogError(ex, "Invalid argument provided");
                return ErrorResponse([ex.Message ], "Invalid argument");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while adding a new category");
                return InternalErrorResponse(ex);
            }
        }
        /// <summary>
        /// Update a category by ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="category"></param>
        /// <returns></returns>

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateDto category)
        {
            if (category == null)
            {
                return ValidationErrorResponse(["Category data is required."]);
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                return ValidationErrorResponse(errors);
            }
            try
            {
                await _categoryService.Update(id, category);
                return SuccessResponse<string>(null, "Category updated successfully");
            }
            catch (KeyNotFoundException)
            {
                logger.LogWarning("Category not found with ID {id}", id);
                return ValidationErrorResponse(["Category not found."]);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while updating category with ID {id}", id);
                return InternalErrorResponse(ex);
            }
        }
        /// <summary>
        /// Delete a category by ID (soft delete / deactivate).
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _categoryService.DeleteAsync(id);
                await _categoryService.SaveChangesAsync();
                return SuccessResponse<string>(null, "Category deactivated successfully");
            }
            catch (KeyNotFoundException)
            {
                logger.LogWarning("Category not found with ID {id}", id);
                return ValidationErrorResponse(["Category not found."]);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while deleting category with ID {id}", id);
                return InternalErrorResponse(ex);
            }
        }
    }
}
