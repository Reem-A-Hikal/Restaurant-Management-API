using Microsoft.AspNetCore.Identity;
using Rest.API.Models;
using Rest.API.Repositories.Implementations;
using Rest.API.Repositories.Interfaces;
using Rest.API.Services.Implementations;
using Rest.API.Services.Interfaces;
using Rest.API.UnitOfWorks.Interfaces;

namespace Rest.API.UnitOfWorks.Implementations
{
    public class UnitOfWork :IUnitOfWork
    {
        private readonly RestDbContext _context;

        private IUserRepository _userRepository;
        private IRepository<User> _userRepository2;
        private IRepository<Address> _addressRepository;
        private IRepository<Category> _categoryRepository;
        private IRepository<Product> _productRepository;
        private IRepository<Order> _orderRepository;
        private IRepository<OrderDetail> _orderDetailRepository;
        private IRepository<Review> _reviewRepository;
        private IRepository<Delivery> _deliveryRepository;

        public UnitOfWork(
            RestDbContext context)
        {
            _context = context;
        }

        public IUserRepository userRepository => _userRepository ??= new UserRepository(_context, _userRepository2);

        public IRepository<Address> addressRepository => _addressRepository ??= new Repository<Address>(_context);

        public IRepository<Category> categoryRepository => _categoryRepository ??= new Repository<Category>(_context);

        public IRepository<Product> productRepository => _productRepository ??= new Repository<Product>(_context);

        public IRepository<Order> orderRepository => _orderRepository ??= new Repository<Order>(_context);

        public IRepository<OrderDetail> orderDetailRepository => _orderDetailRepository ??= new Repository<OrderDetail>(_context);

        public IRepository<Review> reviewRepository => _reviewRepository ??= new Repository<Review>(_context);

        public IRepository<Delivery> deliveryRepository => _deliveryRepository ??= new Repository<Delivery>(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
