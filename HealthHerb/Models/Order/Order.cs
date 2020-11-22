using HealthHerb.Enum;
using HealthHerb.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace HealthHerb.Models.Product
{
    public class Order : Entity
    {
        public string UserId { get; set; }
        public string PaymentId { get; set; }

        [Display(Name ="First name")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Apartment { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }

        [Display(Name = "Postal code")]
        public string PostalCode { get; set; }

        [Display(Name = "Order number")]
        public int OrderNumber { get; set; }

        [Display(Name = "Order status")]
        public OrderStatus OrderStatus { get; set; }
        public ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
