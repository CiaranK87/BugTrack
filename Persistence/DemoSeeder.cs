using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Persistence
{
    public static class DemoSeeder
    {
        public static async Task SeedDemoUserAsync(UserManager<AppUser> userManager, ILogger logger, CancellationToken cancellationToken = default)
        {
            const string demoEmail = "Demo@bugtrack.com";

            var demoUser = await userManager.FindByEmailAsync(demoEmail);

            if (demoUser == null)
            {
                demoUser = new AppUser
                {
                    DisplayName = "Demo User",
                    Email = demoEmail,
                    UserName = "Demo User",
                    GlobalRole = "Guest"
                };

                var result = await userManager.CreateAsync(demoUser, "Dem0Pa$$");

                if (result.Succeeded)
                    logger.LogInformation("Demo user created successfully");
                else
                    logger.LogWarning("Demo user creation failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));

                return;
            }

            if (demoUser.GlobalRole != "Guest")
            {
                demoUser.GlobalRole = "Guest";
                await userManager.UpdateAsync(demoUser);
            }
        }
    }
}
