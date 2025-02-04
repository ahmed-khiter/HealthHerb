﻿using HealthHerb.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HealthHerb.Models.Product
{
    public class Product : Entity
    {
        public string Name { get; set; }
        public ICollection<Image> Images { get; set; }
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public float Discount { get; set; }
        public string Description { get; set; }
        public bool Appear { get; set; }

    }
}
