using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rest.API.Dtos.CategoryDtos;
using Rest.API.Models;
using Rest.API.Services.Interfaces;

namespace Rest.API.Controllers
{
    /// <summary>
    /// Controller for managing categories.
    /// </summary>
    [Route("api/category")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CategoryController : ControllerBase
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
            this._categoryService = categoryService;
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
                return Ok(categories);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while getting all categories");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
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
                if (category == null)
                {
                    return NotFound();
                }
                return Ok(category);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while getting category with ID {id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                if (category == null)
                {
                    return BadRequest("Category cannot be null");
                }
                var createdCategory = await _categoryService.AddAsync(category);
                await _categoryService.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = createdCategory.CategoryId }, createdCategory);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while adding a new category");
                return StatusCode(500, "Internal server error");
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                if (category == null)
                {
                    return BadRequest("Category cannot be null");
                }
                await _categoryService.Update(id, category);
                await _categoryService.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while updating category with ID {id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
        /// <summary>
        /// DeActivate a category by ID
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
                return Ok("DeActivate a category successfully");
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogWarning(ex, "Category not found");
                return NotFound(new
                {
                    Message = "Category not found",
                    Details = ex.Message
                });
            }
            catch (NullReferenceException ex)
            {
                logger.LogError(ex, "Null reference encountered");
                return StatusCode(500, new
                {
                    Message = "Internal server error - null reference",
                    Details = ex.Message
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while deleting category with ID {id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error: " + ex.Message);
            }
        }
    }
}
