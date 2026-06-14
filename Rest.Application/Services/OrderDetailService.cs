using AutoMapper;
using Rest.Application.Dtos.OrderDetailsDtos;
using Rest.Application.Dtos.ReviewDtos;
using Rest.Application.Interfaces.IRepositories;
using Rest.Application.Interfaces.IServices;
using Rest.Domain.Entities;

namespace Rest.Application.Services
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IMapper _mapper;


        public OrderDetailService(IOrderDetailRepository orderDetailRepository, IMapper mapper)
        {
            _orderDetailRepository = orderDetailRepository;
            _mapper = mapper;
        }

        public async Task AddOrderDetailAsync(OrderDetailDto dto)
        {
            //await _orderDetailRepository.AddAsync(orderDetail);
            await _orderDetailRepository.SaveChangesAsync();
        }

        public Task DeleteOrderDetailAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<OrderDetailDto>> GetAllOrderDetailsAsync()
        {
            var details = await _orderDetailRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<OrderDetailDto>>(details);
        }

        public async Task<OrderDetailDto> GetOrderDetailByIdAsync(int id)
        {
            var detail = await _orderDetailRepository.GetByIdAsync(id);
            return _mapper.Map<OrderDetailDto>(detail);
        }

        public Task UpdateOrderDetailAsync(OrderDetailDto orderDetail)
        {
            throw new NotImplementedException();
        }
    }
}
