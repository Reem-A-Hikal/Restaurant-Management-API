using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rest.Application.Dtos.ProductDtos;
using Rest.Application.Interfaces.IServices;
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

        /// <summary>
        /// Initializes a new instance of the ProductController
        /// </summary>
        /// <param name="productService"></param>
        public ProductController(IProductService productService)
        {
            this.productService = productService;
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
            var products = await productService.GetAllProductsAsync();
            return SuccessResponse(products, "Products retrieved successfully");
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
            var products = await productService.GetPaginatedProductsWithFilterAsync(pageIndex, pageSize, searchTerm, selectedFilter);
            return SuccessResponse(products, "Products retrieved successfully");
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
            var product = await productService.GetProductByIdAsync(id);
            return SuccessResponse(product, "Product retrieved successfully");
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
            var products = await productService.GetProductsByCategoryAsync(categoryId);
            return SuccessResponse(products, "Products retrieved successfully");
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
            if (!ModelState.IsValid)
                return ValidationErrorResponse(ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));

            var productId = await productService.CreateProductAsync(productDto);
            return CreatedResponse(nameof(GetProductById), new { id = productId }, productDto, "Product created successfully");
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
                return ValidationErrorResponse(ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));

            await productService.UpdateProductAsync(id, productDto);
            return SuccessResponse(id, "Product updated successfully");
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
            await productService.DeleteProductAsync(id);
            return SuccessResponse(id, "Product archived successfully");
        }
    }
}
