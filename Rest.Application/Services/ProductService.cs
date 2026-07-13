using AutoMapper;
using Rest.Application.Dtos.ProductDtos;
using Rest.Application.Interfaces;
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
        private readonly IImageUploadService _imageUploadService;

        /// <summary>
        /// Constructor for ProductService
        /// </summary>
        /// <param name="productRepository"> The product repository for data access</param>
        /// <param name="mapper"> The AutoMapper instance for mapping between DTOs and entities</param>
        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IImageUploadService imageUploadService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _imageUploadService = imageUploadService;
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
        /// <param name="dto"> The product data transfer object containing the product details</param>
        /// <returns> The created product</returns>
        public async Task<int> CreateProductAsync(ProductCreateDto dto)
        {
            if (dto.DiscountPercent > dto.AllowedDiscountPercent)
                throw new ValidationException("Discount percent cannot exceed allowed discount percent.");

            var product = Product.Create(
                name: dto.Name,
                price: dto.Price,
                preparationTime: dto.PreparationTime,
                categoryId: dto.CategoryId,
                description: dto.Description,
                imageUrl: dto.ImageUrl,
                calories: dto.Calories,
                allowedDiscountPercent: dto.AllowedDiscountPercent,
                discountPercent: dto.DiscountPercent,
                isPromoted: dto.IsPromoted);

            await _unitOfWork.ProductRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return product.ProductId;
        }

        /// <summary>
        /// Updates an existing product by its ID
        /// </summary>
        /// <param name="id"> The ID of the product to update</param>
        /// <param name="dto"> The updated product data transfer object</param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"> Thrown when the product with the specified ID is not found</exception>
        public async Task UpdateProductAsync(int id, ProductUpdateDto dto)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id)
                ?? throw new NotFoundException("Product", id);

            if (dto.CategoryId.HasValue && dto.CategoryId != product.CategoryId)
                _ = await _unitOfWork.CategoryRepository.GetByIdAsync(dto.CategoryId.Value)
                    ?? throw new NotFoundException("Category", dto.CategoryId.Value);

            var oldImageUrl = product.ImageUrl;

            product.UpdateDetails(
                dto.Name, dto.Description, dto.ImageUrl,
                dto.PreparationTime, dto.Calories, dto.CategoryId);

            if (dto.Price.HasValue)
                product.UpdatePrice(dto.Price.Value);

            if (dto.DiscountPercent.HasValue || dto.AllowedDiscountPercent.HasValue)
                product.SetDiscount(
                    dto.DiscountPercent ?? product.DiscountPercent,
                    dto.AllowedDiscountPercent);

            if (dto.IsPromoted.HasValue)
            {
                if (dto.IsPromoted.Value) product.Promote();
                else product.Unpromote();
            }

            if (dto.Status.HasValue)
            {
                switch (dto.Status.Value)
                {
                    case ProductStatus.Available: product.Activate(); break;
                    case ProductStatus.Unavailable: product.Deactivate(); break;
                    case ProductStatus.Archived:
                        throw new BusinessException("Use the delete endpoint to archive a product.");
                }
            }

            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(dto.ImageUrl)
                && dto.ImageUrl != oldImageUrl
                && !string.IsNullOrWhiteSpace(oldImageUrl))
            {
                _imageUploadService.Delete(oldImageUrl);
            }
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

            product.Archive();

            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
