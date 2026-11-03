using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest.Application.Dtos.CheckoutConfigDtos
{
    public class CheckoutConfigDto
    {
        public decimal DeliveryFee { get; set; }
        public bool AcceptingOrders { get; set; }
        public OrderWindowDto OrderWindow { get; set; } = new();
        public DeliveryEstimateDto DeliveryEstimate { get; set; } = new();
    }

    public class OrderWindowDto
    {
        /// <summary>"HH:mm", Cairo time.</summary>
        public string OpensAt { get; set; } = string.Empty;
        public string ClosesAt { get; set; } = string.Empty;
    }

    public class DeliveryEstimateDto
    {
        public int MinMinutes { get; set; }
        public int MaxMinutes { get; set; }


    }
}
