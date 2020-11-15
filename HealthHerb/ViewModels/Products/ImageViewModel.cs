using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthHerb.ViewModels.Products
{
    public class ImageViewModel
    {
        public string ImageId { get; set; }
        public string CurrentImage { get; set; }
        public IFormFile NewImage { get; set; }
    }
}
