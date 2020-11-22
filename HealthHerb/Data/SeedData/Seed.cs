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
                List<string> countries = new List<string>() {
                            " Andorra",
                            " United Arab Emirates              "," Afghanistan",
                            " Antigua and Barbuda",
                            " Albania",
                            " Armenia",
                            " Angola",
                            " Argentina",
                            " Austria",
                            " Australia",
                            " Azerbaijan",
                            " Bosnia and Herzegovina",
                            " Barbados",
                            " Bangladesh",
                            " Belgium",
                            " Burkina Faso",
                            " Bulgaria ",
                            " Bahrain",
                            " Burundi",
                            " Benin ",
                            " Brunei Darussalam",
                            " Bolivia (Plurinational State of)",
                            " Brazil",
                            " Bahamas",
                            " Bhutan",
                            " Botswana",
                            " Belarus",
                            " Belize",
                            " Canada",
                            " Democratic Republic of the Congo",
                            " Central African Republic",
                            " Congo",
                            " Switzerland",
                            " Côte d'Ivoire",
                            " Chile",
                            " Cameroon",
                            " China",
                            " Colombia",
                            " Costa Rica",
                            " Cuba",
                            " Cape Verde",
                            " Cyprus",
                            " Czech Republic",
                            " Germany",
                            " Djibouti",
                            " Denmark",
                            " Dominica",
                            " Dominican Republic",
                            " Algeria",
                            " Ecuador",
                            " Estonia",
                            " Egypt",
                            " Eritrea",
                            " Spain",
                            " Ethiopia",
                            " Finland",
                            " Fiji",
                            " Micronesia (Federated States of)",
                            " France",
                            " Gabon",
                            " United Kingdom of Great Britain anNorthern Ireland",
                            " Grenada",
                            " Georgia",
                            " Ghana",
                            " Gambia",
                            " Guinea",
                            " Equatorial Guinea",
                            " Greece",
                            " Guatemala",
                            " Guinea-Bissau",
                            " Guyana",
                            " Honduras",
                            " Croatia",
                            " Haiti",
                            " Hungary",
                            " Indonesia",
                            " Ireland",
                            " Israel",
                            " India",
                            " Iraq",
                            " Iran (Islamic Republic of)",
                            " Iceland",
                            " Italy",
                            " Jamaica",
                            " Jordan",
                            " Japan ",
                            " Kenya ",
                            " Kyrgyzstan",
                            " Cambodia",
                            " Kiribati",
                            " Comoros  ",
                            " Saint Kitts and Nevis ",
                            " Democratic People's Republic of Koea",
                            " Republic of Korea ",
                            " Kuwait  ",
                            " Kazakhstan                        ",
                            " Lao People's Democratic Republic",
                            " Lebanon ",
                            " Saint Lucia",
                            " Liechtenstein ",
                            " Sri Lanka ",
                            " Liberia  ",
                            " Lesotho   ",
                            " Lithuania ",
                            " Luxembourg  ",
                            " Latvia",
                            " Libyan Arab Jamahiriya",
                            " Morocco  ",
                            " Monaco",
                            " Republic of Moldova",
                            " Montenegro",
                            " Madagascar",
                            " Marshall Islands",
                            " Mali",
                            " Myanmar ",
                            " Mongolia ",
                            " Mauritania",
                            " Malta ",
                            " Mauritius ",
                            " Maldives",
                            " Malawi ",
                            " Mexico ",
                            " Malaysia ",
                            " Mozambique",
                            " Namibia ",
                            " Niger  ",
                            " Nigeria ",
                            " Nicaragua",
                            " Netherlands",
                            " Norway ",
                            " Nepal ",
                            " Nauru ",
                            " New Zealand                       ",
                            " Oman ",
                            " Panama",
                            " Peru",
                            " Papua New Guinea",
                            " Philippines",
                            " Pakistan ",
                            " Poland ",
                            " Portugal",
                            " Palau  ",
                            " Paraguay ",
                            " Qatar",
                            " Romania",
                            " Serbia ",
                            " Russian Federation",
                            " Rwanda ",
                            " Saudi Arabia ",
                            " Solomon Islands  ",
                            " Seychelles",
                            " Sudan ",
                            " Sweden ",
                            " Singapore ",
                            " Slovenia ",
                            " Slovakia ",
                            " Sierra Leone ",
                            " San Marino ",
                            " Senegal",
                            " Somalia ",
                            " Suriname ",
                            " South Sudan ",
                            " Sao Tome and Principe",
                            " El Salvador   ",
                            " Syrian Arab Republic ",
                            " Swaziland ",
                            " Chad ",
                            " Togo ",
                            " Thailand ",
                            " Tajikistan ",
                            " Timor-Leste ",
                            " Turkmenistan",
                            " Tunisia ",
                            " Tonga ",
                            " Turkey ",
                            " Trinidad and Tobago",
                            " Tuvalu",
                            " United Republic of Tanzania",
                            " Ukraine",
                            " Uganda",
                            " United States of America ",
                            " Uruguay",
                            " Uzbekistan",
                            " Saint Vincent and the Grenadines",
                            " Venezuela (Bolivarian Republic of)",
                            " Viet Nam ",
                            " Vanuatu",
                            " Samoa ",
                            " Yemen ",
                            " South Africa ",
                            " Zambia",
                            " Zimbabwe",};
                List<ShippingPrice> record = new List<ShippingPrice>();
                foreach (var country in countries)
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
