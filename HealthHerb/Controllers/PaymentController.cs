using HealthHerb.Authorization;
using HealthHerb.Data;
using HealthHerb.Enum;
using HealthHerb.Interface;
using HealthHerb.Models;
using HealthHerb.Models.Product;
using HealthHerb.Models.Settings;
using HealthHerb.Models.User;
using HealthHerb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthHerb.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ICrud<Models.Product.Product> productCrud;
        private readonly ICrud<Cart> cartCrud;
        private readonly ICrud<Models.Product.Order> orderCrud;
        private readonly ICrud<CouponUsed> couponUsedCrud;
        private readonly ICrud<Models.Product.Coupon> crud;
        private readonly ICrud<OrderProduct> orderProductCrud;
        private readonly ICrud<PaymentSetting> paymentSettingCrud;
        private readonly ICrud<ShippingPrice> shippingPriceCrud;
        private readonly AppDbContext context;
        private readonly SignInManager<BaseUser> signInManager;
        private readonly UserManager<BaseUser> userManager;

        public PaymentController
        (
            ICrud<Models.Product.Product> productCrud,
            ICrud<Cart> cartCrud,
            ICrud<Models.Product.Order> orderCrud,
            ICrud<CouponUsed> couponUsedCrud,
            ICrud<Models.Product.Coupon> crud,
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
            this.couponUsedCrud = couponUsedCrud;
            this.crud = crud;
            this.orderProductCrud = orderProductCrud;
            this.paymentSettingCrud = paymentSettingCrud;
            this.shippingPriceCrud = shippingPriceCrud;
            this.context = context;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }


        [HttpGet]
        public async Task<IActionResult> PrepareOrder()
        {
            if (!signInManager.IsSignedIn(User))
            {
                return Redirect("/account/login");
            }
            var model = new PrepareProductViewModel();
            var countries = await shippingPriceCrud.GetAll();
            ViewData["Countries"] = new SelectList(countries.OrderBy(m => m.Country), "Id", "Country");

            var user = await userManager.GetUserAsync(User);
            var carts = await context.Carts.Where(m => m.UserId.Equals(user.Id))
                    .Include(m => m.Product).ThenInclude(m => m.Images).ToListAsync();
            List<Models.Product.Product> products = new List<Models.Product.Product>();
           
            foreach (var item in carts)
            {
                products.Add(await productCrud.GetById(m => m.Id == item.ProductId));
                model.TotalPrice += item.TotalPrice*item.Quantity;
                model.Quantity.Add(item.Quantity);
            }
            

            if (products.Count() == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            model.Products = products;
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.Email = user.Email;
            model.PhoneNumber = user.PhoneNumber;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> PrepareOrder(PrepareProductViewModel model, string stripeToken)
        {
            var userback = await userManager.GetUserAsync(User);
            if (!model.ShouldProcess)
            {
                var prepareModel = new PrepareProductViewModel();
                prepareModel = model;

                var countries = await shippingPriceCrud.GetAll();
                ViewData["Countries"] = new SelectList(countries.OrderBy(m => m.Country), "Id", "Country");
               
                var cartsback = await context.Carts.Where(m => m.UserId.Equals(userback.Id))
                    .Include(m => m.Product).ThenInclude(m => m.Images).ToListAsync();

                List<Models.Product.Product> products = new List<Models.Product.Product>();
                foreach (var item in cartsback)
                {
                    products.Add(await productCrud.GetById(m => m.Id == item.ProductId));
                    prepareModel.Quantity.Add(item.Quantity);
                }

                if (model.Country != null)
                {
                    var shippingPrice = await shippingPriceCrud.GetFirst(m => m.Id.Equals(model.Country));

                    if (shippingPrice != null)
                    {
                        prepareModel.Country = shippingPrice.Id;
                        prepareModel.ShippingPrice = shippingPrice.Price;
                        prepareModel.TotalPrice += shippingPrice.Price;
                    }
                }

                if (model.DiscountCode != null)
                {
                    var record = await crud.GetById(m => m.Code.Equals(model.DiscountCode));
                    if (record != null)
                    {
                        if (record.ManyUsed > 0 && (DateTime.Now > record.Start || DateTime.Now < record.End))
                        {
                            var copupon = await couponUsedCrud.GetFirst(m => m.CouponId.Equals(record.Id) && m.BaseUserId.Equals(userback.Id));
                            if (!copupon.Invalid)
                            {
                                record.ManyUsed -= 1;
                                await crud.Update(record);
                                await couponUsedCrud.Add(new CouponUsed
                                {
                                    BaseUserId = userback.Id,
                                    CouponId = record.Id
                                });
                                model.TotalPrice = prepareModel.TotalPrice - (prepareModel.TotalPrice * (decimal)(record.Amount / 100) + prepareModel.ShippingPrice);
                            }
                           
                        }
                        else
                        {
                            ViewData["invalid"] = "invalid coupon or you used this coupon before ";
                        }
                    }
                    else
                    {
                        ViewData["invalid"] = "invalid coupon or you used this coupon before ";
                    }
                }
                prepareModel.ShouldProcess = true;
                prepareModel.Products = products;
                return View(prepareModel);
            }



            var user = await userManager.GetUserAsync(User);
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var options = new ChargeCreateOptions
            {
                Amount = (long?)(model.TotalPrice * 100),
                Currency = "GBP",
                Source = stripeToken,
                ReceiptEmail = user.Email,
            };
            var service = new ChargeService();
            Charge charge = service.Create(options);

            var carts = await cartCrud.GetAll(m => m.UserId.Equals(user.Id));
            var country = await shippingPriceCrud.GetById(m => m.Id.Equals(model.Country));
            var order = await orderCrud.Add(new Models.Product.Order
            {
                PaymentId = charge.Id,
                UserId = userback.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                Apartment = model.Apartment,
                City = model.City,
                Country = country.Country,
                PostalCode = model.PostalCode,
                PhoneNumber = model.PhoneNumber,
                OrderStatus = OrderStatus.Processing,
                Email = userback.Email,
                OrderNumber = new Random().Next(1,9999),
            });

            foreach (var cart in carts)
            {
                var orders = await orderProductCrud.Add(new OrderProduct
                {
                    OrderId = order.Id,
                    Order = order,
                    PaymentId = order.PaymentId,
                    ProductId = cart.ProductId,
                    ProductPrice = cart.TotalPrice/cart.Quantity,
                    TotalPrice = cart.TotalPrice,
                    Quantity = cart.Quantity,
                    BaseUser = user,
                    UserId = user.Id,
                });
                await cartCrud.Delete(m => m.UserId.Equals(user.Id));
            }

            var couponHasDone = await crud.GetById(m => m.Code.Equals(model.DiscountCode));
            if (couponHasDone != null)
            {
                var endcoupon = await couponUsedCrud.GetFirst(m => m.CouponId.Equals(couponHasDone.Id) && m.BaseUserId.Equals(userback.Id));
                endcoupon.Invalid = true;
                await couponUsedCrud.Update(endcoupon);
            }
            ViewData["Success"] = $"Your order with number{order.OrderNumber} has been received and is now being processed we have sent you  a receipt to your registered email";
            return RedirectToAction("OrderHistory", "Home");
        }
    }
}
