using HealthHerb.Authorization;
using HealthHerb.Models.User;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthHerb.Data.SeedData
{
    public static class SeedUsers
    {
        public static void Seed(UserManager<BaseUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            string email = "Dev@gmail.com";
            string firstName = "Emad";
            string lastName = "Ahmed";

            if (userManager.FindByEmailAsync(email).Result == null)
            {
                var user = new BaseUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    NormalizedEmail = email.ToUpper(),
                    NormalizedUserName = email.ToUpper(),
                    FirstName = firstName,
                    LastName = lastName,
                    FullName = $"{firstName} {lastName}"
                };

                var result = userManager.CreateAsync(user, "qaz2wsxedc").Result;

                if (result.Succeeded)
                {
                    if (!roleManager.RoleExistsAsync(Role.Admin).Result)
                    {
                        var role = new IdentityRole
                        {
                            Name = Role.Admin
                        };

                        roleManager.CreateAsync(role).Wait();
                    }

                    userManager.AddToRoleAsync(user, Role.Admin).Wait();
                }
            }
        }
    }
}
