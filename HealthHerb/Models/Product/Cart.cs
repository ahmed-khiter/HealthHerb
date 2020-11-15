using HealthHerb.Models.Base;
using HealthHerb.Models.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HealthHerb.Models.Product
{
    public class Cart : Entity
    {
        public string ProductId { get; set; }
        public string UserId { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
        public Product Product { get; set; }
        public BaseUser BaseUser { get; set; }
    }
}
