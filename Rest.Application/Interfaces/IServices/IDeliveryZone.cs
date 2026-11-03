using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest.Application.Interfaces.IServices
{
    public interface IDeliveryZone
    {
        string Governorate { get; }
        IReadOnlyList<string> Areas { get; }
        bool IsDeliverable(string? city, string? governorate);
    }
}
