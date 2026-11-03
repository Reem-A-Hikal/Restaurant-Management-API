using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Rest.Application.Dtos.CheckoutConfigDtos;
using Rest.Application.Interfaces.IServices;
using Rest.Application.Options;

namespace Rest.API.Controllers
{
    [Route("api/checkout-config")]
    public class CheckoutConfigController : BaseController
    {
        private readonly OrderPricingOptions _pricing;
        private readonly DeliveryEstimateOptions _estimate;
        private readonly IKitchenSchedule _kitchen;

        public CheckoutConfigController(
            IOptions<OrderPricingOptions> pricing,
            IOptions<DeliveryEstimateOptions> estimate,
            IKitchenSchedule kitchen)
        {
            _pricing = pricing.Value;
            _estimate = estimate.Value;
            _kitchen = kitchen;
        }

        /// <summary>
        /// What the cart needs before the customer presses confirm.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [ResponseCache(NoStore = true)] // acceptingOrders changes with the clock
        public IActionResult Get() =>
            SuccessResponse(new CheckoutConfigDto
            {
                DeliveryFee = _pricing.DeliveryFee,
                AcceptingOrders = _kitchen.IsAcceptingOrders(DateTime.UtcNow),
                OrderWindow = new OrderWindowDto
                {
                    OpensAt = _kitchen.OpensAt.ToString(@"hh\:mm"),
                    ClosesAt = _kitchen.ClosesAt.ToString(@"hh\:mm"),
                },
                DeliveryEstimate = new DeliveryEstimateDto
                {
                    MinMinutes = _estimate.MinMinutes,
                    MaxMinutes = _estimate.MaxMinutes,
                },
            });
    }
}
