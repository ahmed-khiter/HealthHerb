using HealthHerb.Models.Base;
using HealthHerb.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthHerb.Models.Product
{
    public class CouponUsed : Entity
    {
        public string BaseUserId { get; set; }
        public string CouponId { get; set; }
        public bool Invalid { get; set; }
        public BaseUser BaseUser { get; set; }
        public Coupon Coupon { get; set; }
    }
}
