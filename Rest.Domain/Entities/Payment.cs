using Rest.Domain.Entities.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Rest.Domain.Entities
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public PaymentMethod Method { get; set; }

        [Required]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        // Transaction ID from payment gateway (Stripe, etc.)
        [StringLength(100)]
        public string? TransactionId { get; set; }

        // Gateway response for debugging
        [StringLength(500)]
        public string? GatewayResponse { get; set; }

        public DateTime? PaidAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual required Order Order { get; set; }
    }
}
