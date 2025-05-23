﻿using Rest.API.Models;
using Rest.API.Repositories.Interfaces;
using Rest.API.Services.Interfaces;

namespace Rest.API.Services.Implementations
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailRepository;

        public OrderDetailService(IOrderDetailRepository orderDetailRepository)
        {
            _orderDetailRepository = orderDetailRepository;
        }

        public async Task AddOrderDetailAsync(OrderDetail orderDetail)
        {
            orderDetail.CalculateSubtotal();
            await _orderDetailRepository.AddAsync(orderDetail);
            await _orderDetailRepository.SaveChangesAsync();
        }

        public Task DeleteOrderDetailAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<OrderDetail>> GetAllOrderDetailsAsync()
        {
            return await _orderDetailRepository.GetAllAsync();
        }

        public async Task<OrderDetail> GetOrderDetailByIdAsync(int id)
        {
            return await _orderDetailRepository.GetByIdAsync(id);
        }

        public Task UpdateOrderDetailAsync(OrderDetail orderDetail)
        {
            throw new NotImplementedException();
        }
    }
}
