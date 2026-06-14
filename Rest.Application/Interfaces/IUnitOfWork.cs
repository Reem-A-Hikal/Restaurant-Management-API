using Rest.Application.Interfaces.IRepositories;
using Rest.Domain.Entities;

namespace Rest.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IAddressRepository AddressRepository {get; }
        ICategoryRepository CategoryRepository { get; }
        IProductRepository ProductRepository { get; }
        IOrderRepository OrderRepository { get; }
        IRepository<OrderDetail> OrderDetailRepository { get; }
        IRepository<Review> ReviewRepository { get; }
        IRepository<Delivery> DeliveryRepository { get; }

        Task<int> SaveChangesAsync();
    }
}
