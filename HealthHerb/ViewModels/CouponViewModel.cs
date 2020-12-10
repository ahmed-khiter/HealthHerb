using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HealthHerb.ViewModels
{
    public class CouponViewModel
    {
        public string Id { get; set; }
        public string Code { get; set; }

        [Required]
        [Display(Name = "Discount by percentage")]
        public float Amount { get; set; }

        [Display(Name = "How many used this cupon?"), Required]
        public int ManyUsed { get; set; }

        [Required]
        public DateTime Start { get; set; }

        [Required]
        public DateTime End { get; set; }
    }
}
