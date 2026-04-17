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

        // 🔹 1. Roller oluştur
        foreach (var role in roles)
        {
            var roleExists = await roleManager.RoleExistsAsync(role);
            if (!roleExists)
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // 🔹 2. Admin kullanıcı oluştur
        var adminUserName = "admin";
        var adminEmail = "admin@test.com"; // ⚠️ bunu düzelt
        var adminPassword = "Admin123!";

        var adminUser = await userManager.FindByNameAsync(adminUserName);

        if (adminUser == null)
        {
            adminUser = new AppUser
            {
                FullName = "System Admin",
                UserName = adminUserName,
                Email = adminEmail,
                EmailConfirmed = true,
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}