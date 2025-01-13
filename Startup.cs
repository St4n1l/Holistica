using Holistica.Data;
using Holistica.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

public static class Startup
{
    public static void Initialize()
    {
        // Call the method to create the admin user and role
        CreateAdminUserAndRole();
    }

    private static void CreateAdminUserAndRole()
    {
        // Initialize the database context
        var context = new ApplicationDbContext();

        // Create role and user managers
        var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
        var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

        // Create the Admin role if it doesn't exist
        if (!roleManager.RoleExists("Admin"))
        {
            var role = new IdentityRole { Name = "Admin" };
            roleManager.Create(role);
        }

        // Create the Admin user if it doesn't exist
        var adminUser = userManager.FindByName("admin@example.com");
        if (adminUser == null)
        {
            var user = new ApplicationUser
            {
                UserName = "admin@example.com",
                Email = "admin@example.com"
            };

            string adminPassword = "Admin@123"; // Set a strong password
            var result = userManager.Create(user, adminPassword);

            if (result.Succeeded)
            {
                // Assign the Admin role to the user
                userManager.AddToRole(user.Id, "Admin");
            }
        }
    }
}