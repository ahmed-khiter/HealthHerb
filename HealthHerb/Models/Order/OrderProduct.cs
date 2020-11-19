using HealthHerb.Models.Base;
using HealthHerb.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthHerb.Models.Product
{
    public class OrderProduct : Entity
    {
        public string OrderId { get; set; }
        public string UserId { get; set; }
        public string ProductId { get; set; }
        public Order Order { get; set; }
        public Product Product { get; set; }
        public BaseUser BaseUser { get; set; }
    }
}
