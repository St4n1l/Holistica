using Holistica.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Holistica.Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        string[] roleNames = ["Admin", "User"];

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        var adminUser = new IdentityUser
        {
            UserName = "S74n1l",
            Email = "stanil.h@abv.bg",
            EmailConfirmed = true
        };

        const string adminPassword = "Admin@123";

        var user = await userManager.FindByEmailAsync(adminUser.Email);

        if (user == null)
        {
            var createUserResult = await userManager.CreateAsync(adminUser, adminPassword);
            if (createUserResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
            else
            {
                foreach (var error in createUserResult.Errors)
                {
                    Console.WriteLine($"Error: {error.Description}");
                }
            }
        }

        await using var context =
            new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

        if (context.Products.Any())
        {
            return;
        }

        var products = new List<Product>
        {
            new Product { ProductId = Guid.NewGuid(), Name = "Med1", ImageUrl = "1.webp", Price = 10.99m, Description = "med1" },
            new Product { ProductId = Guid.NewGuid(), Name = "Med2", ImageUrl = "2.webp", Price = 15.99m, Description = "med2" },
            new Product { ProductId = Guid.NewGuid(), Name = "Med3", ImageUrl = "3.webp", Price = 20.99m, Description = "med3" },
        };

        await context.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }
}