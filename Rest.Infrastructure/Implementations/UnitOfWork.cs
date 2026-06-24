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
        private IOrderDetailRepository? _orderDetailRepository;
        private IDeliveryRepository? _deliveryRepository;
        private IPaymentRepository? _paymentRepository;

        private IRepository<Review>? _reviewRepository;


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

        public IOrderDetailRepository OrderDetailRepository =>
        _orderDetailRepository ??= new OrderDetailRepository(_context, new Repository<OrderDetail>(_context));

        public IDeliveryRepository DeliveryRepository =>
            _deliveryRepository ??= new DeliveryRepository(_context, new Repository<Delivery>(_context));

        public IPaymentRepository PaymentRepository =>
            _paymentRepository ??= new PaymentRepository(_context, new Repository<Payment>(_context));
        public IRepository<Review> ReviewRepository =>
        _reviewRepository ??= new Repository<Review>(_context);

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context?.Dispose();
    }
}
