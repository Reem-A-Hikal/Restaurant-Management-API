using Rest.Application.Interfaces;
using Rest.Application.Interfaces.IRepositories;
using Rest.Domain.Entities;
using Rest.Infrastructure.Data;
using Rest.Infrastructure.Implementations.Repositories;

namespace Rest.Infrastructure.Implementations
{
    public class UnitOfWork :IUnitOfWork
    {
        private readonly RestDbContext _context;

        private IUserRepository? _userRepository;
        private IDeliveryPersonRepository? _deliveryPersonRepository;
        private IChefRepository? _chefRepository;
        private IAddressRepository? _addressRepository;
        private ICategoryRepository? _categoryRepository;
        private IProductRepository? _productRepository;
        private IOrderRepository? _orderRepository;

        private IRepository<OrderDetail>? _orderDetailRepository;
        private IRepository<Review>? _reviewRepository;
        private IRepository<Delivery>? _deliveryRepository;


        public UnitOfWork(
            RestDbContext context)
        {
            _context = context;
        }

        public IUserRepository UserRepository =>
            _userRepository ??= new UserRepository(_context);

        public IChefRepository ChefRepository => 
            _chefRepository ??= new ChefRepository(_context);

        public IDeliveryPersonRepository DeliveryPersonRepository =>
            _deliveryPersonRepository ??= new DeliveryPersonRepository(_context);

        public IAddressRepository AddressRepository =>
            _addressRepository ??= new AddressRepository(_context,new Repository<Address>(_context));

        public ICategoryRepository CategoryRepository => 
            _categoryRepository ??= new CategoryRepository(new Repository<Category>(_context), _context);
        
        public IProductRepository ProductRepository =>
        _productRepository ??= new ProductRepository(_context, new Repository<Product>(_context));

        public IOrderRepository OrderRepository =>
         _orderRepository ??= new OrderRepository(_context, new Repository<Order>(_context));

        public IRepository<OrderDetail> OrderDetailRepository =>
        _orderDetailRepository ??= new Repository<OrderDetail>(_context);

        public IRepository<Review> ReviewRepository =>
        _reviewRepository ??= new Repository<Review>(_context);

        public IRepository<Delivery> DeliveryRepository =>
            _deliveryRepository ??= new Repository<Delivery>(_context);

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context?.Dispose();
    }
}
