﻿using AutoMapper;
using Rest.API.Dtos.ProductDtos;
using Rest.API.Models;
using Rest.API.Repositories.Interfaces;
using Rest.API.Services.Interfaces;

namespace Rest.API.Services.Implementations
{
    /// <summary>
    /// Service implementation for product operations
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor for ProductService
        /// </summary>
        /// <param name="productRepository"> The product repository for data access</param>
        /// <param name="mapper"> The AutoMapper instance for mapping between DTOs and entities</param>
        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets all products
        /// </summary>
        /// <returns> List of all products</returns>
        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllWithCatAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        /// <summary>
        /// Gets all available products
        /// </summary>
        /// <returns> List of available products</returns>
        public async Task<IEnumerable<ProductDto>> GetAvailableProductsAsync()
        {
            var products = await _productRepository.GetAvailableProductsAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }
        /// <summary>
        /// Gets a product by its ID
        /// </summary>
        /// <param name="id"> The ID of the product to retrieve</param>
        /// <returns> The product with the specified ID</returns>

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return _mapper.Map<ProductDto>(product);
        }

        /// <summary>
        /// Gets products by category
        /// </summary>
        /// <param name="categoryId"> The ID of the category to filter products by</param>
        /// <returns> List of products in the specified category</returns>

        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _productRepository.GetByCategoryAsync(categoryId);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        /// <summary>
        /// Searches for products by name
        /// </summary>
        /// <param name="searchTerm"> The search term to look for in product names</param>
        /// <returns> List of products matching the search term</returns>
        public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm)
        {
            var products = await _productRepository.SearchProductsAsync(searchTerm);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        /// <summary>
        /// Creates a new product
        /// </summary>
        /// <param name="productDto"> The product data transfer object containing the product details</param>
        /// <returns> The created product</returns>
        public async Task<Product> CreateProductAsync(ProductCreateDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();
            return product;
        }

        /// <summary>
        /// Updates an existing product by its ID
        /// </summary>
        /// <param name="id"> The ID of the product to update</param>
        /// <param name="productDto"> The updated product data transfer object</param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"> Thrown when the product with the specified ID is not found</exception>
        public async Task UpdateProductAsync(int id, ProductUpdateDto productDto)
        {
            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null)
            {
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }
            _mapper.Map(productDto, existingProduct);
            _productRepository.Update(existingProduct);
            await _productRepository.SaveChangesAsync();
        }
        /// <summary>
        /// Deletes a product by its ID
        /// </summary>
        /// <param name="id"> The ID of the product to delete</param>
        /// <returns> Task representing the asynchronous operation</returns>
        public async Task DeleteProductAsync(int id)
        {
            await _productRepository.DeleteAsync(id);
            await _productRepository.SaveChangesAsync();
        }
    }
}
