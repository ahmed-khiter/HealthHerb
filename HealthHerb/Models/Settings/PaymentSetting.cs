using HealthHerb.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HealthHerb.Models.Settings
{
    public class PaymentSetting : Entity
    {
        [Display(Name ="Secret key")]
        public string SecretKey { get; set; }
        [Display(Name ="Publish key")]
        public string PublishKey { get; set; }

    }
}
