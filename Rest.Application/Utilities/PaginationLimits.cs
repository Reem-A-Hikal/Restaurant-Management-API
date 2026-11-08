using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest.Application.Utilities
{
    /// <summary>
    /// One place for paging bounds. Out-of-range values are clamped, not rejected,
    /// so a stale client never gets a 500 for a bad page number.
    /// </summary>
    public class PaginationLimits
    {
        public const int DefaultPageSize = 10;
        public const int MaxPageSize = 50;

        public static (int PageIndex, int PageSize) Normalize(int pageIndex, int pageSize)
        {
            var size = pageSize < 1 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize);
            var index = Math.Max(pageIndex, 1);
            return (index, size);
        }
    }
}
