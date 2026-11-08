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
        /// Admin fallback: records the cash collected for a Delivered order.
        /// Amount is taken from the order; duplicate Completed payments are rejected.
        /// </summary>
        Task<PaymentDto> RecordCashPaymentAsync(int orderId);

        /// <summary>
        /// Refunds the completed payment for an order.
        /// </summary>
        Task<PaymentDto> RefundPaymentAsync(int orderId, string? gatewayResponse = null);

        /// <summary>
        /// Full payment history for an order (audit/support).
        /// </summary>
        Task<IEnumerable<PaymentDto>> GetPaymentHistoryAsync(int orderId, string userId, bool isAdmin);
    }
}
