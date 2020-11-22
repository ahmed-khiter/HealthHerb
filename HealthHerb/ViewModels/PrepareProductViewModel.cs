using HealthHerb.Enum;
using HealthHerb.Models.Product;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HealthHerb.ViewModels
{
    public class PrepareProductViewModel
    {
        public List<Product> Products { get; set; }
        public string UserId { get; set; }
        public decimal PriceAfterDiscount { get; set; }
        public string Email { get; set; }
        public string DiscountCode { get; set; }
        public string PaymentId { get; set; }
        [Display(Name = "First name")]
        public string FirstName { get; set; }
        [Display(Name = "Last name")]
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Apartment { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        [Display(Name = "Phone")]
        public string PhoneNumber { get; set; }
        [Display(Name = "PostalCode")]
        public string PostalCode { get; set; }
        public decimal ShippingPrice { get; set; }
        public bool ShouldProcess { get; set; } = true;
        public decimal TotalPrice { get; set; }
        public string ShippingId { get; set; }
        public List<int> Quantity { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public PrepareProductViewModel()
        {
            Quantity = new List<int>();
        }
    }
}
