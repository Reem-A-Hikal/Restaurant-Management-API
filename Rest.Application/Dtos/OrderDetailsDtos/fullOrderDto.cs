

namespace Rest.Application.Dtos.OrderDetailsDtos
{
    public class fullOrderDto
    {
        public int OrderDetailId { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; } = 1; // Default to 1

        public decimal UnitPrice { get; set; }

        public decimal Subtotal { get; set; }

        public string? SpecialInstructions { get; set; }
    }
}
