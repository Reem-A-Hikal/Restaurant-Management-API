using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest.Application.Dtos.DeliveryZoneDtos
{
    public class DeliveryZoneDto
    {
        public string Governorate { get; set; } = string.Empty;
        public IReadOnlyList<string> Areas { get; set; } = [];
    }
}
