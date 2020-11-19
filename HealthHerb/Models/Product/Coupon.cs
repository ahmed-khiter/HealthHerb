using HealthHerb.Models.Base;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HealthHerb.Models.Product
{
    public class Coupon : Entity
    {
        [Required]
        [Remote(action: "IsCodeInUse", controller:"Coupon")]
        public string Code { get; set; }

        [Column(TypeName = "decimal(18,2)"),Required]
        [Display(Name ="Discount by percentage")]
        public float Amount { get; set; }
        
        [Display(Name ="How many used this cupon?"),Required]
        public int ManyUsed { get; set; }

        [Required]
        public DateTime Start { get; set; }

        [Required]
        public DateTime End { get; set; }
        public bool IsValid { get; set; }

    }
}
