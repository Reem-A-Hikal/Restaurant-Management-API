using AutoMapper;
using Rest.Application.Dtos.PaymentDtos;
using Rest.Application.Interfaces;
using Rest.Application.Interfaces.IServices;
using Rest.Domain.Entities;
using Rest.Domain.Entities.Enums;
using Rest.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaymentDto> RecordCashPaymentAsync(int orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId)
                ?? throw new NotFoundException("Order", orderId);

            if (order.Status != OrderStatus.Delivered)
                throw new BusinessException("Cash can only be recorded for a delivered order.");

            if (await _unitOfWork.PaymentRepository.HasCompletedPaymentAsync(orderId))
                throw new BusinessException("Order already has a completed payment.");

            var payment = Payment.CashCollected(order);

            await _unitOfWork.PaymentRepository.AddAsync(payment);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PaymentDto>(payment);
        }

        public async Task<PaymentDto> RefundPaymentAsync(int orderId, string? gatewayResponse = null)
        {
            var payment = await _unitOfWork.PaymentRepository.GetCompletedPaymentAsync(orderId)
             ?? throw new NotFoundException("No completed payment found for order", orderId);

            payment.Refund(gatewayResponse);

            _unitOfWork.PaymentRepository.Update(payment);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PaymentDto>(payment);
        }

        public async Task<IEnumerable<PaymentDto>> GetPaymentHistoryAsync(int orderId, string userId, bool isAdmin)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId)
                ?? throw new NotFoundException("Order", orderId);

            if (!isAdmin && order.UserId != userId)
                throw new ForbiddenException("You can only view your own payments.");

            var payments = await _unitOfWork.PaymentRepository.GetByOrderIdAsync(orderId);
            return _mapper.Map<PaymentDto[]>(payments);
        }
    }
}
