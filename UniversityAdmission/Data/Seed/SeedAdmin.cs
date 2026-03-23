using Microsoft.AspNetCore.Identity;
using UniversityAdmission.Models.Identity;

namespace UniversityAdmission.Data.Seed
{
    public static class SeedAdmin
    {
        public static async Task SeedAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            const string adminEmail = "admin@admin.com";
            const string adminPassword = "Admin123!";

            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "Admin"
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (!result.Succeeded)
                    throw new Exception(string.Join(
                        Environment.NewLine,
                        result.Errors.Select(e => e.Description)));
            }

            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}