using HealthHerb.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HealthHerb.ViewModels.Order
{
    public class SingleOrderViewModel
    {
        public string UserId { get; set; }
        public string OrderId { get; set; }
        public string ProductsId { get; set; }
        public decimal DiscountCode { get; set; }
        public string PaymentId { get; set; }
        [Display(Name ="First name")]
        public string FirstName { get; set; }
        [Display(Name = "Last name")]
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Apartment { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        [Display(Name ="Phone")]
        public string PhoneNumber { get; set; }
        [Display(Name ="PostalCode")]
        public string PostalCode { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public OrderStatus OrderStatus { get; set; }

    }
}
