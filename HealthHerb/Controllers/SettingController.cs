using HealthHerb.Authorization;
using HealthHerb.Interface;
using HealthHerb.Models;
using HealthHerb.Models.Product;
using HealthHerb.Models.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthHerb.Controllers
{
    [Authorize(Roles = Role.Admin)]

    public class SettingController : Controller
    {
        private readonly ICrud<PaymentSetting> paymentManageCrud;
        private readonly ICrud<Order> orderCrud;
        private readonly ICrud<OrderProduct> orderProductCrud;
        private readonly ICrud<ShippingPrice> shippingPriceCrud;

        public SettingController
        (
            ICrud<PaymentSetting> paymentManageCrud, 
            ICrud<Order> orderCrud,
            ICrud<OrderProduct> orderProductCrud,
            ICrud<ShippingPrice> shippingPriceCrud
        )
        {
            this.paymentManageCrud = paymentManageCrud;
            this.orderCrud = orderCrud;
            this.orderProductCrud = orderProductCrud;
            this.shippingPriceCrud = shippingPriceCrud;
        }

        public async Task<IActionResult> Index()
        {
            var result = await orderCrud.GetAll(new string[] { "OrderProducts"});

            var totalResult = result.Select(m => m.OrderProducts.Select(x => x.TotalPrice).Sum()).SingleOrDefault();
            ViewData["total"] = totalResult;

            var todayResult = result.Where(m => (int)(DateTime.Now - m.CreatedAt).TotalDays == 0)
                             .Select(m => m.OrderProducts
                            .Select(x => x.TotalPrice).Sum()).SingleOrDefault();
            ViewData["today"] = totalResult;

            var monthResult = result.Where(m => (int)(DateTime.Now - m.CreatedAt).TotalDays == 30)
                                .Select(m => m.OrderProducts
                                .Select(x => x.TotalPrice).Sum()).SingleOrDefault();
            ViewData["month"] = monthResult;

            var lastYear = DateTime.Today.AddYears(-1);
            var yearResult = result.Where(m => m.CreatedAt >= lastYear)
                               .Select(m => m.OrderProducts
                               .Select(x => x.TotalPrice).Sum()).SingleOrDefault();
            ViewData["year"] = yearResult;

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> PaymentSetting()
        {
            var payment = await paymentManageCrud.GetById("PaymentSetting");

            if (payment == null)
            {
                payment=  await paymentManageCrud.Add(new PaymentSetting
                            {
                                 Id = "PaymentSetting",
                            });
            }

            return View(payment);
        }

        [HttpPost]
        public async Task<IActionResult> PaymentSetting(PaymentSetting model)
        {
            var payment = await paymentManageCrud.GetById(model.Id);

            if (payment == null)
            {
                return View();
            }
            payment.SecretKey = model.SecretKey;
            payment.PublishKey = model.PublishKey;
            await paymentManageCrud.Update(payment);

            return Redirect("/product/index");
        }
    }
}
