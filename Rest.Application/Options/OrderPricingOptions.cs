using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest.Application.Options
{
    public class OrderPricingOptions
    {
        public const string SectionName = "OrderPricing";

        public decimal DeliveryFee { get; set; } = 50m;
    }
}
