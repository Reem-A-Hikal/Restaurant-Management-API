using Microsoft.AspNetCore.Identity;
using Rest.Domain.Entities;
using Rest.Domain.Exceptions;

namespace Rest.Application.Services
{
    public class UserCreationHelper
    {
        private readonly UserManager<User> _userManager;

        public UserCreationHelper(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task CreateAndAssignRoleAsync(User user, string password, string role)
        {
            var existingUser = await _userManager.FindByEmailAsync(user.Email);
            if (existingUser != null)
                throw new ValidationException("User with this email already exists");

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new ValidationException(result.Errors.Select(e => e.Description));

            var roleResult = await _userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                throw new ValidationException(roleResult.Errors.Select(e => e.Description));
            }
        }
    }
}
