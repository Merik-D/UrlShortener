using Microsoft.AspNetCore.Identity;
using UrlShortener.Repository;

namespace UrlShortener;

public class RoleInitializer
{
    public static async Task Initialize(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        var roleNames = new[] { "user", "admin" };

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                var role = new IdentityRole(roleName);
                await roleManager.CreateAsync(role);
            }
        }
        
        var user = await userManager.FindByEmailAsync("admin@example.com");
        if (user == null)
        {
            user = new ApplicationUser { UserName = "admin@example.com", Email = "admin@example.com" };
            await userManager.CreateAsync(user, "Password123!");
        }

        // Призначення ролі адміну
        if (!await userManager.IsInRoleAsync(user, "admin"))
        {
            await userManager.AddToRoleAsync(user, "admin");
        }
    }
}
