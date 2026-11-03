using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest.Application.Options
{
    public class DeliveryZoneOptions
    {
        public const string SectionName = "DeliveryZone";
        public string Governorate { get; set; } = "Cairo";

        public List<string> Areas { get; set; } = new();
    }
}
