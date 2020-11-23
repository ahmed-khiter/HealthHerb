using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthHerb.Authorization;
using HealthHerb.Data;
using HealthHerb.Data.SeedData;
using HealthHerb.Help;
using HealthHerb.Interface;
using HealthHerb.Models.Settings;
using HealthHerb.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stripe;

namespace HealthHerb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // MVC
            services.AddControllersWithViews(config =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            }).AddRazorRuntimeCompilation();

            // Database
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(Environment.GetEnvironmentVariable("HealthHerb"));
            });

            // Identity
            services
                .AddIdentity<BaseUser, IdentityRole>(
                options=>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredUniqueChars = 0;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // Authorization
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policy.Admin, Policy.AdminPolicy());
            });

            // Dependency Injection
            services.AddScoped<AppDbContext>();
            services.AddScoped(typeof(ICrud<>), typeof(Crud<>));
            services.AddSingleton(typeof(FileManager));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure
        (
            IApplicationBuilder app,
            IWebHostEnvironment env,
            UserManager<BaseUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext context,
            ICrud<PaymentSetting> paymentSettingCrud
        )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            Seed.SeedUser(userManager, roleManager);
            Seed.SeedCountries(context);
            var payment = paymentSettingCrud.GetById("PaymentSetting").Result;
            StripeConfiguration.ApiKey = payment.SecretKey;

            //app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
