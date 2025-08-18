using Rest.Domain.Entities;
using Rest.Domain.Interfaces;
using Rest.Domain.Interfaces.IRepositories;
using Rest.Infrastructure.Data;
using Rest.Infrastructure.Repositories;

namespace Rest.Infrastructure.Implementations
{
    public class UnitOfWork :IUnitOfWork
    {
        private readonly RestDbContext _context;

        private IUserRepository _userRepository;
        private IRepository<User> _userGenericRepository;

        private IAddressRepository _addressRepository;
        private IRepository<Address> _addressGenericRepository;

        private ICategoryRepository _categoryRepository;
        private IRepository<Category> _categoryGenericRepository;


        private IProductRepository _productRepository;
        private IRepository<Product> _productGenericRepository;

        private IOrderRepository _orderRepository;
        private IRepository<Order> _orderGenericRepository;

        private IRepository<OrderDetail> _orderDetailRepository;
        private IRepository<Review> _reviewRepository;
        private IRepository<Delivery> _deliveryRepository;


        public UnitOfWork(
            RestDbContext context)
        {
            _context = context;
        }

        public IUserRepository userRepository
        {
            get
            {
                if (_userGenericRepository == null)
                    _userGenericRepository = new Repository<User>(_context);
                _userRepository = new UserRepository(_context, _userGenericRepository);
                return _userRepository;
            }
        }

        public IAddressRepository addressRepository
        {
            get
            {
                if (_addressGenericRepository == null)
                    _addressGenericRepository = new Repository<Address>(_context);
                _addressRepository = new AddressRepository(_context, _addressGenericRepository);
                return _addressRepository;
            }
        }

        public ICategoryRepository categoryRepository
        {
            get
            {
                if (_categoryGenericRepository == null)
                    _categoryGenericRepository = new Repository<Category>(_context);
                _categoryRepository = new CategoryRepository( _categoryGenericRepository, _context);
                return _categoryRepository;
            }
        }

        public IProductRepository productRepository
        {
            get
            {
                if (_productGenericRepository == null)
                    _productGenericRepository = new Repository<Product>(_context);
                _productRepository = new ProductRepositort(_context, _productGenericRepository);
                return _productRepository;
            }
        }

        public IOrderRepository orderRepository
        {
            get
            {
                if (_orderGenericRepository == null)
                    _orderGenericRepository = new Repository<Order>(_context);
                _orderRepository = new OrderRepository(_context, _orderGenericRepository);
                return _orderRepository;
            }
        }

        public IRepository<OrderDetail> orderDetailRepository
        {
            get
            {
                if (_orderDetailRepository == null)
                    _orderDetailRepository = new Repository<OrderDetail>(_context);
                return _orderDetailRepository;
            }
        }

        public IRepository<Review> reviewRepository
        {
            get
            {
                if (_reviewRepository == null)
                    _reviewRepository = new Repository<Review>(_context);
                return _reviewRepository;
            }
        }

        public IRepository<Delivery> deliveryRepository
        {
            get
            {
                if (_deliveryRepository == null)
                    _deliveryRepository = new Repository<Delivery>(_context);
                return _deliveryRepository;
            }
        }


        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
