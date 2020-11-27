using HealthHerb.Enum;
using HealthHerb.Models.Product;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthHerb.Models.User
{
    public class BaseUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string StripeKey { get; set; }
        public AccountType AccountType { get; set; }
        public CouponUsed CouponUsed { get; set; }

    }
}
