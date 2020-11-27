using HealthHerb.Models;
using HealthHerb.Models.Product;
using HealthHerb.Models.Settings;
using HealthHerb.Models.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HealthHerb.Data
{
    public class AppDbContext : IdentityDbContext<BaseUser>
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Coupon> Discounts { get; set; }
        public DbSet<PaymentSetting> PaymentManages { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<ShippingPrice> ShippingPrices { get; set; }
        public DbSet<CouponUsed> CouponUseds { get; set; }
        public DbSet<FrontEndData> FrontEndDatas { get; set; }
        public DbSet<Image> Images { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) 
                : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
