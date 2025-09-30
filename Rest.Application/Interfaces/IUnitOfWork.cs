using Rest.Application.Interfaces.IRepositories;
using Rest.Domain.Entities;

namespace Rest.Application.Interfaces
{
    public interface IUnitOfWork
    {
        //IAuthService authService { get; }
        //IRepository<User> userRepository {get; }
        IUserRepository userRepository { get; }
        IAddressRepository addressRepository {get; }
        ICategoryRepository categoryRepository { get; }
        IProductRepository productRepository { get; }
        IOrderRepository orderRepository { get; }
        IRepository<OrderDetail> orderDetailRepository { get; }
        IRepository<Review> reviewRepository { get; }
        IRepository<Delivery> deliveryRepository { get; }

        Task<int> SaveChangesAsync();
    }
}
