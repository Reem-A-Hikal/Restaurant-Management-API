using Rest.Application.Dtos.OrderDetailsDtos;
using Rest.Domain.Entities;

namespace Rest.Application.Interfaces.IServices
{
    public interface IOrderDetailService
    {
        Task<IEnumerable<OrderDetailDto>> GetAllOrderDetailsAsync();
        Task<OrderDetailDto> GetOrderDetailByIdAsync(int id);
        Task AddOrderDetailAsync(OrderDetailDto orderDetail);
        Task UpdateOrderDetailAsync(OrderDetailDto orderDetail);
        Task DeleteOrderDetailAsync(int id);
    }
}
