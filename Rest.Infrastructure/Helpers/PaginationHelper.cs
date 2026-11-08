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

            // long math: a huge pageIndex must not overflow int and turn into a negative Skip
            var skip = (int)Math.Min((long)(pageIndex - 1) * pageSize, int.MaxValue);

            var items = await source
                .Skip(skip)
                .Take(size)
                .ToListAsync();

            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
