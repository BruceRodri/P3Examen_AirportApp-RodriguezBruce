using Microsoft.AspNetCore.Identity;

namespace P3Examen_AirportApp.Data;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        string[] roles = { "Administrador", "Cliente" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        if (await roleManager.RoleExistsAsync("Usuario"))
        {
            var oldUsers = await userManager.GetUsersInRoleAsync("Usuario");
            foreach (var oldUser in oldUsers)
            {
                if (!await userManager.IsInRoleAsync(oldUser, "Cliente"))
                {
                    await userManager.AddToRoleAsync(oldUser, "Cliente");
                }
            }
        }

        string emailAdmin = "admin@espe.edu.ec";
        string passwordAdmin = "Admin123*";

        var admin = await userManager.FindByEmailAsync(emailAdmin);

        if (admin == null)
        {
            admin = new IdentityUser
            {
                UserName = emailAdmin,
                Email = emailAdmin,
                EmailConfirmed = true
            };

            await userManager.CreateAsync(admin, passwordAdmin);
        }

        if (!await userManager.IsInRoleAsync(admin, "Administrador"))
        {
            await userManager.AddToRoleAsync(admin, "Administrador");
        }
    }
}
