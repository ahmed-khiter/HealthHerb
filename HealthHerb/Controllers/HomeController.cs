using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HealthHerb.Models;
using HealthHerb.Interface;
using HealthHerb.Models.Product;
using Microsoft.AspNetCore.Identity;
using HealthHerb.Models.User;
using Microsoft.AspNetCore.Authorization;
using HealthHerb.ViewModels.Order;
using HealthHerb.Enum;
using HealthHerb.Models.Settings;
using Microsoft.AspNetCore.Mvc.Rendering;
using Stripe;

namespace HealthHerb.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ICrud<Models.Product.Product> productCrud;
        private readonly ICrud<Cart> cartCrud;
        private readonly ICrud<Models.Product.Order> orderCrud;
        private readonly ICrud<OrderProduct> orderProductCrud;
        private readonly ICrud<PaymentSetting> paymentSettingCrud;
        private readonly ICrud<ShippingPrice> shippingPriceCrud;
        private readonly SignInManager<BaseUser> signInManager;
        private readonly UserManager<BaseUser> userManager;

        public HomeController
        (
            ICrud<Models.Product.Product> productCrud,
            ICrud<Cart> cartCrud, 
            ICrud<Models.Product.Order> orderCrud,
            ICrud<OrderProduct> orderProductCrud,
            ICrud<PaymentSetting> paymentSettingCrud,
            ICrud<ShippingPrice> shippingPriceCrud,
            SignInManager<BaseUser> signInManager,
            UserManager<BaseUser> userManager
        )
        {
            this.productCrud = productCrud;
            this.cartCrud = cartCrud;
            this.orderCrud = orderCrud;
            this.orderProductCrud = orderProductCrud;
            this.paymentSettingCrud = paymentSettingCrud;
            this.shippingPriceCrud = shippingPriceCrud;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await productCrud.GetAll();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> SpecificProduct(string productId)
        {
            var model = await productCrud.GetById(productId);
            return View(model);
        }       

        [HttpGet]
        public async Task<IActionResult> PrepareSingleOrder(string productId)
        {
            if (!signInManager.IsSignedIn(User))
            {
                return Redirect("/account/login");
            }

            var user = await userManager.GetUserAsync(User);            
            var product = await productCrud.GetById(m => m.Id == productId);

            if (product == null)
            {
                return RedirectToAction(nameof(SpecificProduct), new { productId });
            }

            var countries = await shippingPriceCrud.GetAll();
            ViewData["Countries"] =  new SelectList(countries.OrderBy(m => m.Country), "Id", "Country");

            var model = new SingleOrderViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber, 
                Price = product.Price,
                ProductsId = product.Id,
                ProductName = product.Name
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> PrepareSingleOrder(SingleOrderViewModel model, string stripeToken)
        {
            var user = await userManager.GetUserAsync(User);
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Dictionary<string, string> Metadata = new Dictionary<string, string>();
            Metadata.Add("Product", model.ProductName);
            Metadata.Add("Quantity", "500");//TODO: add quantity here 
            var options = new ChargeCreateOptions
            {
                Amount = (long?)(model.Price*100),//TODO: amount will added here from model
                Currency = "GBP",
                Description = model.ProductName,
                Source = stripeToken,
                ReceiptEmail = user.Email,
                Metadata = Metadata
            };
            var service = new ChargeService();
            Charge charge = service.Create(options);

            var order = await orderCrud.Add(new Models.Product.Order
            {
                PaymentId = charge.Id,
                UserId = model.UserId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                Apartment = model.Apartment,
                City = model.City,
                Country = model.Country,
                PostalCode = model.PostalCode,
                PhoneNumber = model.PhoneNumber,
                OrderStatus = OrderStatus.Processing,
                OrderId = (new Random().Next(1, 100000)).ToString()
            });
            var orderProduct = await orderProductCrud.Add(new OrderProduct
            {
                OrderId = order.Id,
                Order = order,
                ProductId = model.PaymentId,
                BaseUser = user,
                UserId = user.Id,
            });

            var product = await productCrud.GetById(model.ProductsId);
            product.Quantity -= 1;
            await productCrud.Update(product);

            ViewData["Success"] = "Your order is going to your address soon";

            return View(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

      
    }
}
