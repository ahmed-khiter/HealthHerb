using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HealthHerb.ViewModels.Products
{
    public class ProductViewModel
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        [DataType(DataType.Currency)]
        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Quantity { get; set; }

        public decimal Discount { get; set; } = 0;

        public string CurrentImage { get; set; }

        [Display(Name = "Product image")]
        public IFormFile Image { get; set; }
    }
}
