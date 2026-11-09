using Microsoft.EntityFrameworkCore;
using Rest.Application.Utilities;

namespace Rest.Infrastructure.Helpers
{
    internal static class PaginationHelper
    {
        internal static async Task<PaginatedList<T>> CreateAsync<T>(
        IQueryable<T> source, int pageIndex, int pageSize)
        {
            var (index, size) = PaginationLimits.Normalize(pageIndex, pageSize);

            var count = await source.CountAsync();

            var skip = (int)Math.Min((long)(index - 1) * size, int.MaxValue);

            var items = await source
                .Skip(skip)
                .Take(size)
                .ToListAsync();

            return new PaginatedList<T>(items, count, index, size);
        }
    }
}
