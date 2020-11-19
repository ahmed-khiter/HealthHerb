using HealthHerb.Interface;
using HealthHerb.Models;
using HealthHerb.Models.Settings;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthHerb.Controllers
{
    public class SettingController : Controller
    {
        private readonly ICrud<PaymentSetting> paymentManageCrud;
        private readonly ICrud<ShippingPrice> shippingPriceCrud;

        public SettingController
        (
            ICrud<PaymentSetting> paymentManageCrud,
            ICrud<ShippingPrice> shippingPriceCrud
        )
        {
            this.paymentManageCrud = paymentManageCrud;
            this.shippingPriceCrud = shippingPriceCrud;
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
            await paymentManageCrud.Update(payment);

            return Redirect("/product/index");
        }
    }
}
