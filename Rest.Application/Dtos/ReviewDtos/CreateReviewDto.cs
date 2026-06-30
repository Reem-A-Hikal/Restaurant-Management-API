using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest.Application.Dtos.ReviewDtos
{
    public class CreateReviewDto
    {
        public int Rating { get; set; }
        public string? Comment { get; set; }

        public int? DeliveryRating { get; set; }

        public int? FoodRating { get; set; }
        public int? ProductId { get; set; }
    }
}
