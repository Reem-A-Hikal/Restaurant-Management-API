using Rest.Application.Interfaces.IRepositories;
using Rest.Domain.Entities;

namespace Rest.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IChefRepository ChefRepository { get; }
        IDeliveryPersonRepository DeliveryPersonRepository { get; }
        IAddressRepository AddressRepository {get; }
        ICategoryRepository CategoryRepository { get; }
        IProductRepository ProductRepository { get; }
        IOrderRepository OrderRepository { get; }
        IOrderDetailRepository OrderDetailRepository { get; }
        IPaymentRepository PaymentRepository { get; }
        IDeliveryRepository DeliveryRepository { get; }
        IReviewRepository ReviewRepository { get; }

        Task<int> SaveChangesAsync();
    }
}
