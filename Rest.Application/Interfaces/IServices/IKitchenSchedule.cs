using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest.Application.Interfaces.IServices
{
    public interface IKitchenSchedule
    {
        TimeSpan OpensAt { get; }
        TimeSpan ClosesAt { get; }
        bool IsAcceptingOrders(DateTime utcNow);
    }
}
