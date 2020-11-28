using HealthHerb.Data;
using HealthHerb.Interface;
using HealthHerb.Models.Product;
using HealthHerb.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly AppDbContext context;
        private readonly UserManager<BaseUser> userManager;

        public CartController
        (
            ICrud<Product> productCrud,
            ICrud<Cart> crud,
            SignInManager<BaseUser> signInManager,
            AppDbContext context,
            UserManager<BaseUser> userManager
        )
        {
            this.productCrud = productCrud;
            this.crud = crud;
            this.signInManager = signInManager;
            this.context = context;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (signInManager.IsSignedIn(User))
            {
                var userId = userManager.GetUserId(User);
                var result = await context.Carts.Where(m => m.UserId == userId)
                                 .Include(m => m.Product).ThenInclude(m => m.Images).ToListAsync();
                return View(result);
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

            ViewData["success"] = "Success Delete";

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

                decimal actuallyPrice = 0;
              
                if (productIsExists == null)
                {
                    if (product.Discount > 0)
                    {
                        actuallyPrice -= (product.Price * (decimal)(product.Discount / 100));
                    }
                    else
                    {
                        actuallyPrice = product.Price;
                    }
                    var productInCart = await crud.Add(new Cart
                                        {
                                            ProductId = productId,
                                            Product = product,
                                            Quantity = 1,
                                            TotalPrice = actuallyPrice,
                                            UserId = userId
                                        });
                }
                else
                {
                    actuallyPrice = product.Price;
                    productIsExists.Quantity += 1;
                    productIsExists.TotalPrice = actuallyPrice * productIsExists.Quantity;
                    await crud.Update(productIsExists);
                }
            }

            ViewData["success"] = $"Product {product.Name} added to cart";

            return RedirectToAction(nameof(Index));
        }
    }
}
