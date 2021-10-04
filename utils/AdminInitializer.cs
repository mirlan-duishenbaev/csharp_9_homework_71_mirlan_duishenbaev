using Microsoft.AspNetCore.Identity;
using MyChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyChat.utils
{
    public class AdminInitializer
    {
        public static async Task SeedAdminUser(
               RoleManager<IdentityRole> _roleManager,
               UserManager<User> _userManager)
        {
            string adminEmail = "admin@admin.com";
            string adminLogin = "Admin";
            string adminPassword = "password";

            string path = "/Files/" + "Admin.jfif";

            FileModel file = new FileModel { Name = "Admin.jfif", Path = path };
            string adminAvatar = file.Path;

            var roles = new[] { "admin", "user" };

            foreach (var role in roles)
            {
                if (await _roleManager.FindByNameAsync(role) is null)
                    await _roleManager.CreateAsync(new IdentityRole(role));
            }

            if (await _userManager.FindByNameAsync(adminEmail) == null)
            {
                User admin = new User { Email = adminEmail, UserName = adminLogin, Avatar = adminAvatar };
                IdentityResult result = await _userManager.CreateAsync(admin, adminPassword);

                if (result.Succeeded)
                    await _userManager.AddToRoleAsync(admin, "admin");
            }
        }
    }
}
