using Microsoft.Extensions.Options;
using Rest.Application.Interfaces.IServices;
using Rest.Application.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest.Application.Services
{
    public class KitchenSchedule : IKitchenSchedule
    {
        private readonly KitchenHoursOptions _options;
        private readonly TimeZoneInfo _zone;

        public KitchenSchedule(IOptions<KitchenHoursOptions> options)
        {
            _options = options.Value;

            if(_options.OrdersCloseAt <= _options.OrdersOpenAt)
                throw new InvalidOperationException(
                    "KitchenHours: OrdersCloseAt must be later than OrdersOpenAt (the window cannot cross midnight).");

            _zone = TimeZoneInfo.FindSystemTimeZoneById(_options.TimeZoneId);
        }
        public TimeSpan OpensAt => _options.OrdersOpenAt;

        public TimeSpan ClosesAt => _options.OrdersCloseAt;

        public bool IsAcceptingOrders(DateTime utcNow)
        {
            var utc = DateTime.SpecifyKind(utcNow, DateTimeKind.Utc);
            var local = TimeZoneInfo.ConvertTimeFromUtc(utc, _zone).TimeOfDay;

            return local >= _options.OrdersOpenAt && local < _options.OrdersCloseAt;
        }
    }
}
