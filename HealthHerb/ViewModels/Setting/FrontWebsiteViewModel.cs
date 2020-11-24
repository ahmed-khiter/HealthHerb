using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthHerb.ViewModels.Setting
{
    public class FrontWebsiteViewModel
    {
        public string Id { get; set; }
        public string BigTitle { get; set; }
        public string HeaderText { get; set; }
        public string Text { get; set; }

        public string CurrentImage { get; set; }
        public IFormFile Image { get; set; }
    }
}
