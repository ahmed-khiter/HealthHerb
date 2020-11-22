using HealthHerb.Authorization;
using HealthHerb.Help;
using HealthHerb.Models;
using HealthHerb.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthHerb.Data.SeedData
{
    public static class Seed
    {
        public static void SeedUser(UserManager<BaseUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            string email = "Dev@gmail.com";
            string firstName = "Emad";
            string lastName = "Ahmed";

            if (!roleManager.RoleExistsAsync(Role.Admin).Result)
            {
                var role = new IdentityRole
                {
                    Name = Role.Admin
                };

                roleManager.CreateAsync(role).Wait();
            }

            if (!roleManager.RoleExistsAsync(Role.Consumer).Result)
            {
                var role = new IdentityRole
                {
                    Name = Role.Consumer
                };

                roleManager.CreateAsync(role).Wait();
            }

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
                    FullName = $"{firstName} {lastName}",
                    AccountType = Enum.AccountType.Admin,
                    PhoneNumber = "01100811024",
                };

                var result = userManager.CreateAsync(user, "_Aa123456789").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, Role.Admin).Wait();
                }
            }
        }

        public static void SeedCountries(AppDbContext context)
        {
            var result = context.ShippingPrices.ToListAsync().Result;
            if (result.Count()==0)
            {
                List<ShippingPrice> record = new List<ShippingPrice>();
                foreach (var country in Countries.CountryList())
                {
                    record.Add(new ShippingPrice
                    {
                        Country = country,
                        Price =0,
                    });
                }

                context.ShippingPrices.AddRangeAsync(record).Wait();
                context.SaveChangesAsync().Wait();
                context.PaymentManages.Add(new Models.Settings.PaymentSetting
                {
                    Id = "PaymentSetting",
                    PublishKey = "here you must enter publish key to make success payment",
                    SecretKey = "here you must enter secret key to make success payment",
                    CreatedAt = DateTime.Now
                });
                context.SaveChanges();
            }
        }
    }
}
