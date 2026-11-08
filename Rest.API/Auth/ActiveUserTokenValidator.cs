using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Rest.Domain.Entities;
using Rest.Domain.Entities.Enums;
using System.Security.Claims;

namespace Rest.API.Auth
{
    /// <summary>
    /// A valid signature is not enough: reject tokens of users who were suspended,
    /// deactivated or soft-deleted after the token was issued.
    /// (FindByIdAsync goes through the Deleted query filter, so deleted users => null.)
    /// </summary>
    public static class ActiveUserTokenValidator
    {
        public static async Task ValidateAsync(TokenValidatedContext context)
        {
            var userId = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) { context.Fail("Invalid token."); return; }

            var users = context.HttpContext.RequestServices.GetRequiredService<UserManager<User>>();
            var user = await users.FindByIdAsync(userId);

            if (user is null || user.Status != UserStatus.Active)
                context.Fail("Account is not active.");
        }
    }
}
