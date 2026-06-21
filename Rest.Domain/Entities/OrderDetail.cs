namespace Rest.Domain.Entities
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;

        /// <summary>
        /// Price snapshot taken from Product.GetDiscountedPrice() at the moment
        /// this line item was created. Never recalculated afterwards — this is
        /// what the customer was actually charged, regardless of later price changes.
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Computed column in DB: Quantity * UnitPrice
        /// </summary>
        public decimal Subtotal { get; private set; }
        public string? SpecialInstructions { get; set; }

        // Navigation properties
        public virtual required Order Order { get; set; }
        public virtual required Product Product { get; set; }
    }
}
