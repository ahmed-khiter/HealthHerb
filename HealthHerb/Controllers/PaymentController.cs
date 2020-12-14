using HealthHerb.Authorization;
using HealthHerb.Data;
using HealthHerb.Enum;
using HealthHerb.Interface;
using HealthHerb.Models;
using HealthHerb.Models.Product;
using HealthHerb.Models.Settings;
using HealthHerb.Models.User;
using HealthHerb.Payment;
using HealthHerb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        private readonly ICrud<Models.Product.Coupon> couponCrud;
        private readonly IHttpContextAccessor httpContextAccessor;
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
            ICrud<Models.Product.Coupon> couponCrud,
            IHttpContextAccessor httpContextAccessor,
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
            this.couponCrud = couponCrud;
            this.httpContextAccessor = httpContextAccessor;
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
                model.TotalPrice += item.TotalPrice;
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

            PaymentSetting paymentCredential = await context.PaymentManages.FirstOrDefaultAsync();
            PayPal paypal = new PayPal(httpContextAccessor, context, paymentCredential.IsLive);

            ViewData["OrderId"] = await paypal.CreateOrder(decimal.Round(model.TotalPrice, 2, MidpointRounding.AwayFromZero), "GBP");
            ViewData["ClientId"] = paymentCredential.ClientId;
            ViewData["ClientToken"] = HttpContext.Request.Cookies["client_token"] ?? await paypal.GenerateClientToken();
            ViewData["Currency"] = "GBP";

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> PrepareOrder(PrepareProductViewModel model, string paypalOrderId, bool capture)
        {
            var userback = await userManager.GetUserAsync(User);

            PaymentSetting paymentCredential = await context.PaymentManages.FirstOrDefaultAsync();
            PayPal paypal = new PayPal(httpContextAccessor, context, paymentCredential.IsLive);

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
                    var action = await CheckCoupon(model, userback.Id);
                    if (action>0)
                    {
                        model.TotalPrice = action + prepareModel.ShippingPrice;
                    }
                    else
                    {
                        ViewData["invalid"] = "invalid coupon or you used this coupon before ";
                    }
                }

                prepareModel.ShouldProcess = true;
                prepareModel.Products = products;

                ViewData["OrderId"] = await paypal.CreateOrder(decimal.Round(prepareModel.TotalPrice, 2, MidpointRounding.AwayFromZero), "GBP");
                ViewData["ClientId"] = paymentCredential.ClientId;
                ViewData["ClientToken"] = HttpContext.Request.Cookies["client_token"] ?? await paypal.GenerateClientToken();
                ViewData["Currency"] = "GBP";

                return View(prepareModel);
            }

            var user = await userManager.GetUserAsync(User);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (capture)
            {
                if (!await paypal.Capture(paypalOrderId))
                {
                    return Redirect("/home/error");
                }
            }

            if (!await paypal.IsPayed(paypalOrderId))
            {
                return Redirect("/home/error");
            }

            var carts = await cartCrud.GetAll(m => m.UserId.Equals(user.Id));
            var country = await shippingPriceCrud.GetById(m => m.Id.Equals(model.Country));
            var order = await orderCrud.Add(new Models.Product.Order
            {
                PaymentId = paypalOrderId,
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

            await DoneCheckCoupon(model, userback.Id);

            var couponHasDone = await crud.GetById(m => m.Code.Equals(model.DiscountCode));
            if (couponHasDone != null)
            {
                var endcoupon = await couponUsedCrud.GetFirst(m => m.CouponId.Equals(couponHasDone.Id) && m.BaseUserId.Equals(userback.Id));
                endcoupon.Invalid = true;
                await couponUsedCrud.Update(endcoupon);
            }
            TempData["SuccessPayment"] = $"Your order with number{order.OrderNumber} has been received and is now being processed we have sent you  a receipt to your registered email";
            return RedirectToAction("OrderHistory", "Home");
        }

        private async Task<decimal> CheckCoupon(PrepareProductViewModel model,string userId)
        {
            var record = await couponCrud.GetById(m => m.Code.Equals(model.DiscountCode));

            if (record == null)
            {
                return 0;
            }

            if (record.ManyUsed > 0 && (DateTime.Now > record.Start || DateTime.Now < record.End))
            {
                var copupon = await couponUsedCrud.GetFirst(m => m.CouponId.Equals(record.Id) 
                && m.BaseUserId.Equals(userId));

                if (copupon == null)
                {
                    model.TotalPrice = model.TotalPrice - (model.TotalPrice * (decimal)(record.Amount / 100));
                    return model.TotalPrice;
                }
                return 0;

            }
            else
            {
                return 0;
            }
        }

        private async Task DoneCheckCoupon(PrepareProductViewModel model, string userId)
        {
            var record = await couponCrud.GetById(m => m.Code.Equals(model.DiscountCode));
            if (record == null)
            {
                return;
            }
            if (record.ManyUsed > 0 && (DateTime.Now > record.Start || DateTime.Now < record.End))
            {
                var copupon = await couponUsedCrud.GetFirst(m => m.CouponId.Equals(record.Id)
                && m.BaseUserId.Equals(userId));

                if (copupon == null)
                {
                    record.ManyUsed -= 1;
                    await crud.Update(record);
                    await couponUsedCrud.Add(new CouponUsed
                    {
                        BaseUserId = userId,
                        CouponId = record.Id
                    });
                }

            }
        }
    }
}
