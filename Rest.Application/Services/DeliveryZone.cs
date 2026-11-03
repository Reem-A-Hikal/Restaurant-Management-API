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
    public class DeliveryZone : IDeliveryZone
    {
        private readonly HashSet<string> _lookup;

        public string Governorate { get; }
        public IReadOnlyList<string> Areas { get; }

        public DeliveryZone(IOptions<DeliveryZoneOptions> options)
        {
            var config = options.Value;

            Areas = config.Areas
                .Select(a => a.Trim())
                .Where(a => a.Length > 0)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (Areas.Count == 0)
                throw new InvalidOperationException("DeliveryZone: Areas must not be empty.");

            Governorate = config.Governorate.Trim();
            _lookup = new HashSet<string>(Areas, StringComparer.OrdinalIgnoreCase);
        }
        public bool IsDeliverable(string? city, string? governorate) =>
            string.Equals(governorate?.Trim(), Governorate, StringComparison.OrdinalIgnoreCase)
            && !string.IsNullOrWhiteSpace(city)
            && _lookup.Contains(city.Trim());
    }
}
