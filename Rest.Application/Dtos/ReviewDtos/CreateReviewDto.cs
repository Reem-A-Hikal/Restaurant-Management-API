using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest.Application.Dtos.ReviewDtos
{
    public class CreateReviewDto
    {
        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string? Comment { get; set; }

        [Range(1, 5)]
        public int? DeliveryRating { get; set; }

        [Range(1, 5)]
        public int? FoodRating { get; set; }
        public int? ProductId { get; set; }
    }
}
