using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest.Application.Dtos.DashboardDtos
{
    public class TopDishDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
        public string ImageUrl { get; set; }
    }
}
