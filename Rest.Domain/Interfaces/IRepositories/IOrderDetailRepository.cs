using Rest.Domain.Entities;

namespace Rest.Domain.Interfaces.IRepositories
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        Task<IEnumerable<OrderDetail>> GetByOrderIdAsync(int orderId);
    }
}
