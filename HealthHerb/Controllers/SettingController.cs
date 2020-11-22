using HealthHerb.Authorization;
using HealthHerb.Help;
using HealthHerb.Interface;
using HealthHerb.Models;
using HealthHerb.Models.Product;
using HealthHerb.Models.Settings;
using HealthHerb.ViewModels.Setting;
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
        private readonly ICrud<FrontEndData> frontendDataCrud;
        private readonly ICrud<OrderProduct> orderProductCrud;
        private readonly ICrud<ShippingPrice> shippingPriceCrud;
        private readonly FileManager fileManager;

        public SettingController
        (
            ICrud<PaymentSetting> paymentManageCrud,
            ICrud<Order> orderCrud,
            ICrud<FrontEndData> frontendDataCrud,
            ICrud<OrderProduct> orderProductCrud,
            ICrud<ShippingPrice> shippingPriceCrud,
            FileManager fileManager

        )
        {
            this.paymentManageCrud = paymentManageCrud;
            this.orderCrud = orderCrud;
            this.frontendDataCrud = frontendDataCrud;
            this.orderProductCrud = orderProductCrud;
            this.shippingPriceCrud = shippingPriceCrud;
            this.fileManager = fileManager;
        }

        public async Task<IActionResult> Index()
        {
            var result = await orderCrud.GetAll(new string[] { "OrderProducts" });

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
                payment = await paymentManageCrud.Add(new PaymentSetting
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


        [HttpGet]
        public async Task<IActionResult> FrontWebsite()
        {
            var frontEndData = await frontendDataCrud.GetById("frontendDataSetting");

            if (frontEndData == null)
            {
                frontEndData = await frontendDataCrud.Add(new FrontEndData
                {
                    Id = "frontendDataSetting",
                });
            }

            var model = new FrontWebsiteViewModel()
            {
                Id = frontEndData.Id,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> FrontWebsite(FrontWebsiteViewModel model)
        {
            var record = await frontendDataCrud.GetById(model.Id);

            if (record == null)
            {
                return View();
            }

            record.Header = model.HeaderText;
            record.Text = model.Text;

            if (model.Image == null)
            {
                record.Image = model.CurrentImage;
            }
            else
            {
                record.Image = fileManager.Upload(model.Image);
                fileManager.Delete(model.CurrentImage);
            }
            await frontendDataCrud.Update(record);

            return Redirect("/product/index");

        }
    }
}
