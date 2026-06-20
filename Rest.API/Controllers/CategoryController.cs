using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Rest.Application.Dtos.CategoryDtos;
using Rest.Application.Interfaces.IServices;
using Swashbuckle.AspNetCore.Annotations;

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

        /// <summary>
        /// Constructor for CategoryController.
        /// </summary>
        /// <param name="categoryService"></param>
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// get all categories
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        [SwaggerOperation(Summary = "Get all categories")]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return SuccessResponse(categories, "Categories retrieved successfully");
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
        [SwaggerOperation(Summary = "Get paginated categories with optional filtering")]
        public async Task<IActionResult> GetPaginated(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = "",
            [FromQuery] string? selectedFilter = "All")
        {
            var categories = await _categoryService.GetPaginatedCatsWithFilterAsync(
                pageIndex, pageSize, searchTerm, selectedFilter);

            return SuccessResponse(categories, "Categories retrieved successfully");
        }
        /// <summary>
        /// get category by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("getcategory/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            return SuccessResponse(category, "Category retrieved successfully");
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

            var createdCategory = await _categoryService.AddAsync(dto);
            return CreatedResponse(
                nameof(GetById),
                new { id = createdCategory.CategoryId },
                createdCategory,
                "Category created successfully");
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
           
            await _categoryService.UpdateAsync(id, request);
            return SuccessResponse<string>(null, "Category updated successfully");
        }
        /// <summary>
        /// Delete a category by ID (soft delete / deactivate).
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Archive(int id)
        {
            await _categoryService.ArchiveAsync(id);
            return SuccessResponse<string>(null, "Category archived successfully");
        }
    }
}
