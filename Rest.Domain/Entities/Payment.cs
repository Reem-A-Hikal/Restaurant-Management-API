using Rest.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest.Domain.Entities
{
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
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
        [ForeignKey("OrderId")]
        public virtual required Order Order { get; set; }
    }
}
