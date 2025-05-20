using Rest.API.Dtos.ProductDtos;
using Rest.API.Models;

namespace Rest.API.Services.Interfaces
{
    /// <summary>
    /// Service interface for product operations
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Retrieves all products
        /// </summary>
        /// <returns> A collection of products</returns>
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();

        /// <summary>
        /// Retrieves a product by its ID
        /// </summary>
        /// <param name="id"> The ID of the product</param>
        /// <returns> The product with the specified ID</returns>
        Task<ProductDto> GetProductByIdAsync(int id);

        /// <summary>
        /// Retrieves products by category ID
        /// </summary>
        /// <param name="categoryId"> The ID of the category</param>
        /// <returns> A collection of products in the specified category</returns>
        Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId);

        /// <summary>
        /// Retrieves Available Products 
        /// </summary>
        /// <returns> A collection of Available Products </returns>
        Task<IEnumerable<ProductDto>> GetAvailableProductsAsync();

        /// <summary>
        /// Search for products by name
        /// </summary>
        /// <param name="searchTerm"> The search term to look for in product names</param>
        /// <returns> A collection of products that match the search term</returns>
        Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm);

        /// <summary>
        /// Creates a new product
        /// </summary>
        /// <param name="productDto"> The product data transfer object containing the product details</param>
        /// <returns> The created product</returns>
        Task<Product> CreateProductAsync(ProductCreateDto productDto);

        /// <summary>
        /// Updates an existing product
        /// </summary>
        /// <param name="id"> The ID of the product to update</param>
        /// <param name="productDto"> The product data transfer object containing the updated product details</param>
        /// <returns> The updated product</returns>
        Task UpdateProductAsync(int id, ProductUpdateDto productDto);

        /// <summary>
        /// Deletes a product by its ID
        /// </summary>
        /// <param name="id"> The ID of the product to delete</param>
        /// <returns> Task representing the asynchronous operation</returns>
        Task DeleteProductAsync(int id);
    }
}
