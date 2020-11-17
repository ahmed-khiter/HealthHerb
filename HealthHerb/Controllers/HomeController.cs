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

namespace HealthHerb.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ICrud<Product> productCrud;
        private readonly ICrud<Cart> cartCrud;
        private readonly ICrud<Order> orderCrud;
        private readonly ICrud<OrderProduct> orderProductCrud;
        private readonly SignInManager<BaseUser> signInManager;
        private readonly UserManager<BaseUser> userManager;

        public HomeController
        (
            ICrud<Product> productCrud,
            ICrud<Cart> cartCrud, 
            ICrud<Order> orderCrud,
            ICrud<OrderProduct> orderProductCrud,
            SignInManager<BaseUser> signInManager,
            UserManager<BaseUser> userManager
        )
        {
            this.productCrud = productCrud;
            this.cartCrud = cartCrud;
            this.orderCrud = orderCrud;
            this.orderProductCrud = orderProductCrud;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

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
        public async Task<IActionResult> PrepareSingleOrder(SingleOrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.GetUserAsync(User);

            var order = await orderCrud.Add(new Order
            {
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
            return View();
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
