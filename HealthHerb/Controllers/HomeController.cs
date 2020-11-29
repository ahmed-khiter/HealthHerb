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
        private readonly AppDbContext context;
        private readonly UserManager<BaseUser> userManager;

        public HomeController
        (
            ICrud<Models.Product.Product> productCrud,
            AppDbContext context,
            UserManager<BaseUser> userManager
        )
        {
            this.productCrud = productCrud;
            this.context = context;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await productCrud.GetAll(m=>m.Quantity>0&&m.Appear , new string[] { "Images" });
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> SpecificProduct(string productId)
        {
            var model = await productCrud.GetById(productId, new string[] { "Images" } );
            return View(model);
        }

        [HttpGet]
        public IActionResult OrderHistory()
        {
            var userId = userManager.GetUserId(User);
            var model = context.Orders
                .Where(m => m.UserId.Equals(userId))
                .Include(m => m.OrderProducts)
                .ThenInclude(m => m.Product)
                .ThenInclude(m=>m.Images);

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
