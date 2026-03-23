using Microsoft.AspNetCore.Identity;

namespace UniversityAdmission.Data.Seed
{
    public static class SeedRoles
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new IdentityRole("User"));
        }
    }
}