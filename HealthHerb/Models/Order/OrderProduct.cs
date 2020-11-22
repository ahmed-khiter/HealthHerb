using HealthHerb.Models.Base;
using HealthHerb.Models.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HealthHerb.Models.Product
{
    public class OrderProduct : Entity
    {
        public string OrderId { get; set; }
        public string UserId { get; set; }
        public string ProductId { get; set; }
        public string PaymentId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ProductPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
        public int Quantity { get; set; }
        public Order Order { get; set; }
        public Product Product { get; set; }
        public BaseUser BaseUser { get; set; }
    }
}
