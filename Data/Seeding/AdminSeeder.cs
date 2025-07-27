using Microsoft.AspNetCore.Identity;
using UniCodeProject.API.DataModels;

namespace UniCodeProject.API.Data.Seeding
{
    public static class AdminSeeder
    {
        public static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var email = "admin@unicode.com";
            var password = "Admin123!";

            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FirstName = "System",
                    LastName = "Administrator"
                };

                var result = await userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    throw new Exception("Unsuccessful attempt to create an admin profile: " +
                                        string.Join("; ", result.Errors.Select(e => e.Description)));
                }
            }

            if (!await userManager.IsInRoleAsync(user, "Administrator"))
            {
                await userManager.AddToRoleAsync(user, "Administrator");
            }
        }
    }
}