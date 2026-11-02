using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest.Application.Options
{
    public class KitchenHoursOptions
    {
        public const string SectionName = "KitchenHours";

        public string TimeZoneId { get; set; } = "Africa/Cairo";

        public TimeSpan OrdersOpenAt { get; set; } = TimeSpan.FromHours(18);
        public TimeSpan OrdersCloseAt { get; set; } = TimeSpan.FromHours(21);
    }
}
