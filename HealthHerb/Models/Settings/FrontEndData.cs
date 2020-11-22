using HealthHerb.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthHerb.Models.Settings
{
    public class FrontEndData : Entity
    {
        public string Header { get; set; }
        public string Text { get; set; }

        public string Image { get; set; }
    }
}
