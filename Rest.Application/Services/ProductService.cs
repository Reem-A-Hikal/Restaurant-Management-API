using AutoMapper;
using Rest.Application.Dtos.ProductDtos;
using Rest.Application.Interfaces;
using Rest.Application.Interfaces.IRepositories;
using Rest.Application.Interfaces.IServices;
using Rest.Application.Utilities;
using Rest.Domain.Entities;
using Rest.Domain.Entities.Enums;
using Rest.Domain.Exceptions;

namespace Rest.Application.Services
{
    /// <summary>
    /// Service implementation for product operations
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor for ProductService
        /// </summary>
        /// <param name="productRepository"> The product repository for data access</param>
        /// <param name="mapper"> The AutoMapper instance for mapping between DTOs and entities</param>
        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets all products
        /// </summary>
        /// <returns> List of all products</returns>
        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _unitOfWork.ProductRepository.GetAllWithCatAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<PaginatedList<ProductDto>> GetPaginatedProductsWithFilterAsync(int pageIndex, int pageSize, string? searchTerm, string? selectedFilter)
        {
            var paginated = await _unitOfWork.ProductRepository.GetPaginatedAsync(pageIndex, pageSize, searchTerm, selectedFilter);

            var dtos = _mapper.Map<List<ProductDto>>(paginated.Items);
            return new PaginatedList<ProductDto>(dtos, paginated.TotalItems, pageIndex, pageSize);
            
        }

        /// <summary>
        /// Gets a product by its ID
        /// </summary>
        /// <param name="id"> The ID of the product to retrieve</param>
        /// <returns> The product with the specified ID</returns>

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id)
                ?? throw new NotFoundException("Product", id);
            return _mapper.Map<ProductDto>(product);
        }

        /// <summary>
        /// Gets products by category
        /// </summary>
        /// <param name="categoryId"> The ID of the category to filter products by</param>
        /// <returns> List of products in the specified category</returns>
        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
        {
            _ = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryId) 
                ?? throw new NotFoundException($"Category", categoryId);

            var products = await _unitOfWork.ProductRepository.GetByCategoryAsync(categoryId);
            return products.Any() ? _mapper.Map<IEnumerable<ProductDto>>(products) : [];
        }

        /// <summary>
        /// Creates a new product
        /// </summary>
        /// <param name="productDto"> The product data transfer object containing the product details</param>
        /// <returns> The created product</returns>
        public async Task<int> CreateProductAsync(ProductCreateDto productDto)
        {
            if (productDto.DiscountPercent > productDto.AllowedDiscountPercent)
                throw new ValidationException("Discount percent cannot exceed allowed discount percent.");

            var product = _mapper.Map<Product>(productDto);
            await _unitOfWork.ProductRepository.AddAsync(product);
            await _unitOfWork.ProductRepository.SaveChangesAsync();
            return product.ProductId;
        }

        /// <summary>
        /// Updates an existing product by its ID
        /// </summary>
        /// <param name="id"> The ID of the product to update</param>
        /// <param name="productDto"> The updated product data transfer object</param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"> Thrown when the product with the specified ID is not found</exception>
        public async Task UpdateProductAsync(int id, ProductDto productDto)
        {
            var existingProduct = await _unitOfWork.ProductRepository.GetByIdAsync(id)
                ?? throw new NotFoundException("Product", id);

            if (productDto.DiscountPercent > productDto.AllowedDiscountPercent)
                throw new ValidationException("Discount percent cannot exceed allowed discount percent.");

            existingProduct.Name = productDto.Name;
            existingProduct.Description = productDto.Description;
            existingProduct.Price = productDto.Price;
            existingProduct.IsPromoted = productDto.IsPromoted;
            existingProduct.Calories = productDto.Calories;
            existingProduct.CategoryId = productDto.CategoryId;
            existingProduct.ImageUrl = productDto.ImageUrl;
            existingProduct.PreparationTime = productDto.PreparationTime;
            existingProduct.Status = productDto.Status;
            existingProduct.AllowedDiscountPercent = productDto.AllowedDiscountPercent;
            existingProduct.DiscountPercent = productDto.DiscountPercent;

            _unitOfWork.ProductRepository.Update(existingProduct);
            await _unitOfWork.ProductRepository.SaveChangesAsync();
        }
        /// <summary>
        /// Archive a product by its ID
        /// </summary>
        /// <param name="id"> The ID of the product to archived</param>
        /// <returns> Task representing the asynchronous operation</returns>
        public async Task DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id)
                    ?? throw new NotFoundException("Product", id);

            product.Status = ProductStatus.Archived;
            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.ProductRepository.SaveChangesAsync();
        }
    }
}
