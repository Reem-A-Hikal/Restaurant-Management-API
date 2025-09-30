using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rest.Application.Dtos.ProductDtos;
using Rest.Application.IServices;
using Swashbuckle.AspNetCore.Annotations;

namespace Rest.API.Controllers
{
    /// <summary>
    /// Controller for managing products.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : BaseController
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
                return SuccessResponse(products, "Products retrieved successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting all products");
                return InternalErrorResponse(ex, "An error occurred while retrieving products");
            }
        }

        /// <summary>
        /// Gets paginated products with optional filtering
        /// </summary>
        /// <param name="pageIndex">Page index (default is 1)</param>
        /// <param name="pageSize">Page size (default is 6)</param>
        /// <param name="searchTerm">Search term for filtering by product name (optional)</param>
        /// <param name="selectedFilter">Selected filter (default is "All")</param>
        /// <returns>Paginated list of products</returns>
        [HttpGet("paginated")]
        [SwaggerOperation(Summary = "Get paginated products with optional filtering")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns paginated list of products")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> GetPaginatedProductsWithFilter(
            int pageIndex = 1, int pageSize = 8,
            string? searchTerm = null,
            string? selectedFilter = "All")
        {
            try
            {
                var products = await productService.GetPaginatedProductsWithFilterAsync(pageIndex, pageSize, searchTerm, selectedFilter);
                return SuccessResponse(products, "Products retrieved successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting paginated products with filter");
                return InternalErrorResponse(ex, "An error occurred while retrieving paginated products");
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
                return SuccessResponse(product, "Product retrieved successfully");

            }
            catch (KeyNotFoundException ex)
            {
                logger.LogWarning(ex, "Product not found with ID: {ProductId}", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting product with ID: {ProductId}", id);
                return InternalErrorResponse(ex, "An error occurred while retrieving the product");
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
                return SuccessResponse(products, "Products retrieved successfully");
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogWarning(ex, "Category not found with ID: {CategoryId}", categoryId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting products for category ID: {CategoryId}", categoryId);
                return InternalErrorResponse(ex, "An error occurred while retrieving products by category");
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
                return ErrorResponse(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage), "Invalid product data");
            }
            try
            {
                var productId = await productService.CreateProductAsync(productDto);
                //return CreatedAtAction(nameof(GetProductById),
                //    new { id = product.ProductId },
                //    new { message = "Product created successfully", productId = product.ProductId });
                return CreatedResponse(nameof(GetProductById), new { id = productId },productDto, "Product created successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating product");
                return InternalErrorResponse(ex, "An error occurred while creating the product");
            }
        }

        /// <summary>
        /// Updates an existing product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="productDto">ProductDto</param>
        [HttpPut("EditProduct/{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Update an existing product")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Product updated successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid product data")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Product not found")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden - Admin role required")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> UpdateProduct(int id, ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return ErrorResponse(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage), "Invalid product data");
            }
            try
            {
                await productService.UpdateProductAsync(id, productDto);
                return SuccessResponse(id, "Product updated successfully");
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogWarning(ex, "Product not found with ID: {ProductId}", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating product with ID: {ProductId}", id);
                return InternalErrorResponse(ex, "An error occurred while updating the product");
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
                return SuccessResponse(id, "Product deleted successfully");
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogWarning(ex, "Product not found with ID: {ProductId}", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting product with ID: {ProductId}", id);
                return InternalErrorResponse(ex, "An error occurred while deleting the product");
            }
        }

        #region Old
        ///// <summary>
        ///// Gets available products
        ///// </summary>
        //[HttpGet("available")]
        //[SwaggerOperation(Summary = "Get available products")]
        //[SwaggerResponse(StatusCodes.Status200OK, "Returns list of available products")]
        //[SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        //public async Task<IActionResult> GetAvailableProducts()
        //{
        //    try
        //    {
        //        var products = await productService.GetAvailableProductsAsync();
        //        return Ok(products);
        //    }
        //    catch (Exception ex)
        //    {
        //         logger.LogError(ex, "Error getting available products");
        //        return StatusCode(500, "An error occurred while retrieving available products");
        //    }
        //}

        ///// <summary>
        ///// Searches products by name
        ///// </summary>
        ///// <param name="searchTerm">Search term</param>
        //[HttpGet("search")]
        //[SwaggerOperation(Summary = "Search products by name")]
        //[SwaggerResponse(StatusCodes.Status200OK, "Returns list of matching products")]
        //[SwaggerResponse(StatusCodes.Status400BadRequest, "Search term is required")]
        //[SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        //public async Task<IActionResult> SearchProducts([FromQuery] string searchTerm)
        //{
        //    if (string.IsNullOrWhiteSpace(searchTerm))
        //    {
        //        return BadRequest();
        //    }
        //    try
        //    {
        //        var products = await productService.SearchProductsAsync(searchTerm);
        //        return Ok(products);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(ex, "Error searching products with term: {SearchTerm}", searchTerm);
        //        return StatusCode(500, "An error occurred while searching products");
        //    }
        //}
        #endregion
    }
}
