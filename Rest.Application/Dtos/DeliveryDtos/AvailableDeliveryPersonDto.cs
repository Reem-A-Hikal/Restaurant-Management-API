using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest.Application.Dtos.DeliveryDtos
{
    public class AvailableDeliveryPersonDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string VehicleNumber { get; set; }
    }
}
