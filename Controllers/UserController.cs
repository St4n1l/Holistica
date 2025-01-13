using Holistica.Data;
using Holistica.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace Holistica;
public class UserController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserController()

    {
        var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
        _userManager = new UserManager<ApplicationUser>(userStore);

        var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
        _roleManager = new RoleManager<IdentityRole>(roleStore);
    }

    [HttpPost]
    public async Task<JsonResult> AssignRole(string userId, string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            return Json(new { success = false, message = "Role does not exist." });
        }   

        var result = await _userManager.AddToRoleAsync(userId, roleName);
        if (result.Succeeded)
        {
            return Json(new { success = true, message = "Role assigned successfully." });
        }
        else
        {
            return Json(new { success = false, message = string.Join(", ", result.Errors) });
        }
    }
}