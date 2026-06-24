using Rest.Domain.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Rest.Application.Dtos.PaymentDtos
{
    public class ProcessPaymentDto
    {
        [Required]
        public PaymentMethod Method { get; set; }
        public string? TransactionId { get; set; }
        public string? GatewayResponse { get; set; }
    }
}
