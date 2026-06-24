using Rest.Application.Dtos.PaymentDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest.Application.Interfaces.IServices
{
    public interface IPaymentService
    {
        /// <summary>
        /// Creates a new Payment attempt. Prevents duplicate Completed payments.
        /// </summary>
        Task<PaymentDto> ProcessPaymentAsync(int orderId, ProcessPaymentDto dto);

        /// <summary>
        /// Refunds the completed payment for an order.
        /// </summary>
        Task<PaymentDto> RefundPaymentAsync(int orderId, string? gatewayResponse = null);

        /// <summary>
        /// Full payment history for an order (audit/support).
        /// </summary>
        Task<IEnumerable<PaymentDto>> GetPaymentHistoryAsync(int orderId);
    }
}
