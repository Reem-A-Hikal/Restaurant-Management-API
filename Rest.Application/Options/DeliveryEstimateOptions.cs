using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest.Application.Options
{
    public class DeliveryEstimateOptions
    {
        public const string SectionName = "DeliveryEstimate";

        public int MinMinutes { get; set; } = 35;
        public int MaxMinutes { get; set; } = 50;
    }
}
