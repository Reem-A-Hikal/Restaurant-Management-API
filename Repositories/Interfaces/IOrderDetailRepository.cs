using Rest.API.Models;

namespace Rest.API.Repositories.Interfaces
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        Task<IEnumerable<OrderDetail>> GetByOrderIdAsync(int orderId);
    }
}
