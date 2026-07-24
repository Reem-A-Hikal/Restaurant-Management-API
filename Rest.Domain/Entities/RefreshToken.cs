using Rest.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest.Domain.Entities
{
    /// <summary>
    /// Represents a single refresh token issued to a user session.
    /// TokenHash stores SHA-256 of the raw token — the raw value is never persisted.
    /// </summary>
    public class RefreshToken
    {
        public int Id { get; private set; }
        public string TokenHash { get; private set; } = null!;
        public string UserId { get; private set; } = null!;
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public DateTime? RevokedAt { get; private set; }

        /// <summary>
        /// Hash of the token that replaced this one during rotation.
        /// Null if this token was never rotated (e.g. still active, or revoked via logout/theft).
        /// </summary>
        public string? ReplacedByTokenHash { get; private set; }

        // Navigation property
        public virtual User? User { get; set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsRevoked => RevokedAt.HasValue;
        public bool IsActive => !IsRevoked && !IsExpired;

        public static RefreshToken Create(string userId, string tokenHash, int daysValid)
        {
            if(string.IsNullOrWhiteSpace(userId))
                throw new ValidationException("UserId is required.");

            if (string.IsNullOrWhiteSpace(tokenHash))
                throw new ValidationException("Token hash is required.");

            if(daysValid <= 0)
                throw new ValidationException("Refresh token validity must be a positive number of days.");

            return new RefreshToken
            {
                UserId = userId,
                TokenHash = tokenHash,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(daysValid)
            };
        }

        /// <summary>
        /// Revokes this token. Pass the new token's hash when revoking as part of
        /// rotation, or leave null when revoking due to logout/theft detection.
        /// </summary>
        public void Revoke(string? replacedByTokenHash = null)
        {
            if(IsRevoked)
                throw new BusinessException("Refresh token is already revoked.");

            RevokedAt = DateTime.UtcNow;
            ReplacedByTokenHash = replacedByTokenHash;
        }
    }
}
