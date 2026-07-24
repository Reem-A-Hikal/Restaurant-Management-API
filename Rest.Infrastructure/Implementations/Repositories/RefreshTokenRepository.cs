using Microsoft.EntityFrameworkCore;
using Rest.Application.Interfaces.IRepositories;
using Rest.Domain.Entities;
using Rest.Infrastructure.Data;

namespace Rest.Infrastructure.Implementations.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly RestDbContext _context;

        public RefreshTokenRepository(RestDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(r => r.TokenHash == tokenHash);
        }

        public async Task AddAsync(RefreshToken entity)
            => await _context.RefreshTokens.AddAsync(entity);

        public async Task RevokeAllActiveForUserAsync(string userId)
        {
            var activeTokens = await _context.RefreshTokens
                .Where(r => r.UserId == userId && r.RevokedAt == null)
                .ToListAsync();

            foreach (var token in activeTokens)
            {
                token.Revoke();
            }
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
