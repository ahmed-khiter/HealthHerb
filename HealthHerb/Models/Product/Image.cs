using HealthHerb.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthHerb.Models.Product
{
    public class Image : Entity
    {
        public string Name { get; set; }
        public string ProductId { get; set; }
        public Product Product { get; set; }
    }
}
