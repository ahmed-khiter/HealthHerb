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
        public ProductViewModel()
        {
            CurrentImages = new List<ImageViewModel>();
        }
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        [DataType(DataType.Currency)]
        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Display(Name="Discount by percentage")]
        public float Discount { get; set; } = 0;

        public List<ImageViewModel> CurrentImages { get; set; }

        [Display(Name = "Add collection of images")]
        public List<IFormFile> Images { get; set; }
    }
}
