using Microsoft.EntityFrameworkCore;
using Rest.Application.Utilities;

namespace Rest.Infrastructure.Helpers
{
    internal static class PaginationHelper
    {
        internal static async Task<PaginatedList<T>> CreateAsync<T>(
        IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
