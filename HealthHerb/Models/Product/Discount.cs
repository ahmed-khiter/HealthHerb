using HealthHerb.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HealthHerb.Models.Product
{
    public class Discount : Entity
    {
        [Required]
        public string Code { get; set; }

        [Column(TypeName = "decimal(18,2)"),Required]
        [Display(Name ="How much discount")]
        public decimal Amount { get; set; }
        
        [Display(Name ="How many used this cupon?"),Required]
        public int ManyUsed { get; set; }

        [Required]
        public DateTime Start { get; set; }

        [Required]
        public DateTime End { get; set; }
    }
}
