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
using HealthHerb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using HealthHerb.Enum;
using HealthHerb.Models.Settings;
using Microsoft.AspNetCore.Mvc.Rendering;
using Stripe;
using HealthHerb.Data;
using Microsoft.EntityFrameworkCore;

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
        private readonly AppDbContext context;
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
            AppDbContext context,
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
            this.context = context;
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
        public async Task<IActionResult> OrderHistory()
        {
            var userId = userManager.GetUserId(User);
            var model = context.Orders
                .Where(m => m.UserId.Equals(userId))
                .Include(m => m.OrderProducts)
                .ThenInclude(m => m.Product);

            return View(model);
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
