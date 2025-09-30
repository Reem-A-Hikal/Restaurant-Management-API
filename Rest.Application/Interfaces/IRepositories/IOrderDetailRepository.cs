using Rest.Domain.Entities;

namespace Rest.Application.Interfaces.IRepositories
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        Task<IEnumerable<OrderDetail>> GetByOrderIdAsync(int orderId);
    }
}
