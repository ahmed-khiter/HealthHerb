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
        [Required]
        [Display(Name = "Client Id")]
        public string ClientId { get; set; }

        [Required]
        [Display(Name = "Client Secret")]
        public string ClientSecret { get; set; }
        public string AccessToken { get; set; }
        public int TokenExpireAt { get; set; }

        [Display(Name = "Live Mode")]
        public bool IsLive { get; set; }
    }
}
