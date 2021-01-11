using System;
using System.Linq;
using BasePackageModule2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BasePackageModule2.Data
{
    public static class SeedData
    {
        public static async void InitializeAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // await context.Database.EnsureDeletedAsync();

            // await context.Database.EnsureCreatedAsync();


            // await context.SaveChangesAsync();


            if (!context.Users.Any())
            {

                ApplicationUser user = new ApplicationUser
                {
                    FirstName = "kishor",
                    LastName = "Gujar",
                    Email = "admin@admin.com",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = "admin@admin.com",
                    PhoneNumber = "+919820787902"
                };


                var adminUser = await userManager.CreateAsync(user, "Admin@1234");

                if (adminUser.Succeeded)
                {

                    await roleManager.CreateAsync(new IdentityRole()
                    {
                        Name = "Admin"
                    });
                    await roleManager.CreateAsync(new IdentityRole()
                    {
                        Name = "User"
                    });

                    await userManager.AddToRoleAsync(user, "Admin");
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
