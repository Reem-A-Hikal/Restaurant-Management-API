using Rest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest.Application.Interfaces.IRepositories
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        /// <summary>
        /// Returns the full payment attempt history for an order
        /// (e.g. Failed, Failed, Completed). Used for support/audit views,
        /// not for business-critical checks.
        /// </summary>
        Task<IEnumerable<Payment>> GetByOrderIdAsync(int orderId);

        /// <summary>
        /// Fast existence check used to prevent creating a duplicate Completed
        /// payment for the same order. Translates to a lightweight EXISTS query
        /// rather than loading full payment records — minimizes the race-condition
        /// window, though it does not eliminate it on its own (see PaymentService
        /// for the full guard, which should pair this with a DB-level constraint).
        /// </summary>
        Task<bool> HasCompletedPaymentAsync(int orderId);

        /// <summary>
        /// Returns the most recent successful payment for an order, if any.
        /// Used when processing refunds (need to know what to refund).
        /// </summary>
        Task<Payment?> GetCompletedPaymentAsync(int orderId);
    }
}
