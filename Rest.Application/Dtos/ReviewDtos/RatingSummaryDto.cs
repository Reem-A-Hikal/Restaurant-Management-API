using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest.Application.Dtos.ReviewDtos
{
    public class RatingSummaryDto
    {
        /// <summary>1.0–5.0, rounded to one decimal. 0 when there are no reviews (check Count).</summary>
        public double Average { get; set; }
        public int Count { get; set; }
    }
}
