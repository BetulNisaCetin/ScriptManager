using DbScriptManager.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DbScriptManager.WebUI.Seeds;

public static class IdentitySeed
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

        string[] roles = { "Admin", "Developer" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var adminUserName = "admin";
        var adminPassword = "Admin123!";

        var adminUser = await userManager.FindByNameAsync(adminUserName);

        if (adminUser == null)
        {
            adminUser = new AppUser
            {
                FirstName    = "System",   // YENİ
                LastName     = "Admin",    // YENİ
                UserName     = adminUserName,
                EmailConfirmed = true,
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
                await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}