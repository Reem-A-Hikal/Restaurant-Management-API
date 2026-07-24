using Rest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest.Application.Interfaces.IRepositories
{
    public interface IRefreshTokenRepository
    {

        /// <summary>
        /// Fetches by hash regardless of active/revoked/expired status —
        /// callers need to see revoked tokens too, to detect reuse (theft).
        /// </summary>
        Task<RefreshToken?> GetByTokenHashAsync(string tokenHash);

        Task AddAsync(RefreshToken entity);

        /// <summary>
        /// Revokes every currently-active token for a user. Used when
        /// token reuse is detected, or on "logout everywhere".
        /// </summary>
        Task RevokeAllActiveForUserAsync(string userId);

        Task SaveChangesAsync();
    }
}
