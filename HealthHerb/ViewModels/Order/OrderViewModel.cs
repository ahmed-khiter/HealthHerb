using HealthHerb.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthHerb.ViewModels.Order
{
    public class OrderViewModel
    {
        public string UserId { get; set; }
        public string OrderId { get; set; }
        public List<string> ProductsId { get; set; }
        public decimal DiscountCode { get; set; }
        public string PaymentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Apartment { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string PostalCode { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatus OrderStatus { get; set; }

    }
}
