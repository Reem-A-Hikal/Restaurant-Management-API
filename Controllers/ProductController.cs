using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rest.API.Dtos.ProductDtos;
using Rest.API.Services.Implementations;
using Rest.API.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Rest.API.Controllers
{
    /// <summary>
    /// Controller for managing products.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService productService;
        private readonly ILogger<ProductController> logger;

        /// <summary>
        /// Initializes a new instance of the ProductController
        /// </summary>
        /// <param name="productService"></param>
        /// <param name="logger"></param>
        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            this.productService = productService;
            this.logger = logger;
        }

        /// <summary>
        /// Gets all products
        /// </summary>
        /// <returns></returns>

        [HttpGet("all")]
        [SwaggerOperation(Summary = "Get all products")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns list of all products")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await productService.GetAllProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting all products");
                return StatusCode(500, "An error occurred while retrieving products");
            }
        }
        /// <summary>
        /// Gets a product by ID
        /// </summary>
        /// <param name="id">Product ID</param>
        [HttpGet("GetProduct/{id}")]
        [SwaggerOperation(Summary = "Get product by ID")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns the requested product")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Product not found")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await productService.GetProductByIdAsync(id);
                return Ok(product);

            }catch (Exception ex)
            {
                logger.LogError(ex, "Error getting product with ID: {ProductId}", id);
                return StatusCode(500, "An error occurred while retrieving the product");
            }
        }

        /// <summary>
        /// Gets products by category
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        [HttpGet("category/{categoryId}")]
        [SwaggerOperation(Summary = "Get products by category")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns list of products in the category")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            try
            {
                var products = await productService.GetProductsByCategoryAsync(categoryId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting products for category ID: {CategoryId}", categoryId);
                return StatusCode(500, "An error occurred while retrieving products by category");
            }
        }

        /// <summary>
        /// Gets available products
        /// </summary>
        [HttpGet("available")]
        [SwaggerOperation(Summary = "Get available products")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns list of available products")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> GetAvailableProducts()
        {
            try
            {
                var products = await productService.GetAvailableProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                 logger.LogError(ex, "Error getting available products");
                return StatusCode(500, "An error occurred while retrieving available products");
            }
        }

        /// <summary>
        /// Searches products by name
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search products by name")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns list of matching products")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Search term is required")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> SearchProducts([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest();
            }
            try
            {
                var products = await productService.SearchProductsAsync(searchTerm);
                return Ok(products);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error searching products with term: {SearchTerm}", searchTerm);
                return StatusCode(500, "An error occurred while searching products");
            }
        }

        /// <summary>
        /// Creates a new product
        /// </summary>
        /// <param name="productDto">Product data</param>
        [HttpPost("AddProduct")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Create a new product")]
        [SwaggerResponse(StatusCodes.Status201Created, "Product created successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid product data")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden - Admin role required")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDto productDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {
                var product = await productService.CreateProductAsync(productDto);
                return CreatedAtAction(nameof(GetProductById),
                    new { id = product.ProductId },
                    new { message = "Product created successfully", productId = product.ProductId });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating product");
                return StatusCode(500, new { message = "An error occurred while creating the product", error = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="productDto">Updated product data</param>
        [HttpPut("EditProduct/{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Update an existing product")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Product updated successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid product data")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Product not found")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden - Admin role required")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdateDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {
                await productService.UpdateProductAsync(id, productDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogWarning(ex, "Product not found with ID: {ProductId}", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating product with ID: {ProductId}", id);
                return StatusCode(500, new { message = new { message = "An error occurred while creating the product", error = ex.Message }, error = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a product
        /// </summary>
        /// <param name="id">Product ID</param>
        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Delete a product")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Product deleted successfully")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Product not found")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden - Admin role required")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                await productService.DeleteProductAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogWarning(ex, "Product not found with ID: {ProductId}", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting product with ID: {ProductId}", id);
                return StatusCode(500, "An error occurred while deleting the product");
            }
        }
    }
}
