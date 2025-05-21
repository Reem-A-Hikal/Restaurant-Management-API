using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Rest.API.Dtos.ProductDtos
{
    /// <summary>
    /// A simple DTO for product information.
    /// </summary>
    public class SimpleProductDto
    {
        /// <summary>
        /// The unique identifier for the product.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// The name of the product.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The price of the product.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Is the product available for sale?
        /// </summary>
        public bool IsAvailable { get; set; } = true;
    }
}
