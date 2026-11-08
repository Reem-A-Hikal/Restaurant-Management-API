using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rest.Domain.Entities;

namespace Rest.Infrastructure.Data
{
    public static class DbSeeder
    {
        private static readonly string[] Roles = ["Admin", "Chef", "DeliveryPerson", "Customer"];

        public static async Task SeedAsync(IServiceProvider services)
        {
            var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DbSeeder");
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<User>>();

            await SeedRolesAsync(roleManager);

            var options = services.GetRequiredService<IConfiguration>()
                .GetSection(AdminSeedOptions.SectionName)
                .Get<AdminSeedOptions>() ?? new AdminSeedOptions();

            await SeedAdminAsync(userManager, options, logger);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        private static async Task SeedAdminAsync(
            UserManager<User> userManager, AdminSeedOptions options, ILogger logger)
        {
            if (string.IsNullOrWhiteSpace(options.Email) ||
                string.IsNullOrWhiteSpace(options.Password))
            {
                logger.LogInformation(
                    "AdminSeed is not configured; skipping admin bootstrap.");
                return;
            }

            var email = options.Email.Trim();

            // Never promote or modify an existing account from configuration.
            if (await userManager.FindByEmailAsync(email) is not null)
            {
                logger.LogInformation("Admin bootstrap skipped: user already exists.");
                return;
            }

            var admin = User.Create(
                email: email,
                userName: email,
                fullName: options.FullName,
                phoneNumber: options.PhoneNumber,
                profileImageUrl: null);

            var created = await userManager.CreateAsync(admin, options.Password);
            if (!created.Succeeded)
            {
                // Log the reasons only — never the password.
                logger.LogError("Failed to seed admin: {Errors}",
                    string.Join(", ", created.Errors.Select(e => e.Description)));
                return;
            }

            var inRole = await userManager.AddToRoleAsync(admin, "Admin");
            if (!inRole.Succeeded)
            {
                logger.LogError("Failed to assign Admin role: {Errors}",
                    string.Join(", ", inRole.Errors.Select(e => e.Description)));
                await userManager.DeleteAsync(admin); // don't leave a role-less account
                return;
            }

            logger.LogWarning(
                "Bootstrap admin created. Remove the AdminSeed settings now.");
        }
    }
}
