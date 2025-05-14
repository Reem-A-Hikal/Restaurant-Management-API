using Rest.API.Models;
using Rest.API.Repositories.Interfaces;
using Rest.API.Services.Interfaces;

namespace Rest.API.UnitOfWorks.Interfaces
{
    public interface IUnitOfWork
    {
        //IAuthService authService { get; }
        IRepository<User> userRepository {get; }
        IRepository<Address> addressRepository {get; }
        IRepository<Category> categoryRepository { get; }
        IRepository<Product> productRepository { get; }
        IRepository<Order> orderRepository { get; }
        IRepository<OrderDetail> orderDetailRepository { get; }
        IRepository<Review> reviewRepository { get; }
        IRepository<Delivery> deliveryRepository { get; }

        Task<int> SaveChangesAsync();
    }
}
