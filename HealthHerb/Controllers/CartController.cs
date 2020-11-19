using HealthHerb.Interface;
using HealthHerb.Models.Product;
using HealthHerb.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthHerb.Controllers
{
    public class CartController : Controller
    {
        private readonly ICrud<Product> productCrud;
        private readonly ICrud<Cart> crud;
        private readonly SignInManager<BaseUser> signInManager;
        private readonly UserManager<BaseUser> userManager;

        public CartController
        (
            ICrud<Product> productCrud,
            ICrud<Cart> crud,
            SignInManager<BaseUser> signInManager,
            UserManager<BaseUser> userManager
        )
        {
            this.productCrud = productCrud;
            this.crud = crud;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (signInManager.IsSignedIn(User))
            {
                var userId = userManager.GetUserId(User);
                return View(await crud.GetAll(m => m.UserId == userId, new string[] { "Product" }));
            }

            return Redirect("/Account/Login");
        }

        public async Task<IActionResult> DecreaseItem(string id)
        {
            var record = await crud.GetById(id);

            if (record.Quantity == 1)
            {
                await crud.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                record.Quantity -= 1;
                await crud.Update(record);
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> IncreaseItem(string id)
        {
            var record = await crud.GetById(id);
            record.Quantity += 1;
            await crud.Update(record);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var model = await crud.GetById(id);

            if (model == null)
            {
                return NotFound();
            }

            await crud.Delete(id);

            TempData["success"] = "Success create";

            return Ok();

        }

        [HttpGet]
        public async Task<IActionResult> Create(string productId)
        {
            var product = await productCrud.GetById(productId);

            if (product == null)
            {
                return Redirect("/");
            }

            if (!signInManager.IsSignedIn(User))
            {
                return Redirect("/Account/Login");
            }

            if (signInManager.IsSignedIn(User))
            {
                var userId = userManager.GetUserId(User);
                var productIsExists = await crud.GetById(m => m.ProductId == productId && m.UserId == userId);

                if (productIsExists == null)
                {
                    await crud.Add(new Cart
                    {
                        ProductId = productId,
                        Product = product,
                        Quantity = 1,
                        TotalPrice = product.Price,
                        UserId = userId
                    });
                }
                else
                {
                    productIsExists.Quantity += 1;
                    productIsExists.TotalPrice *= productIsExists.Quantity;
                    await crud.Update(productIsExists);
                }
            }

            TempData["success"] = $"Product {product.Name} added to cart";

            return Redirect("/");
        }
    }
}
