using HealthHerb.Authorization;
using HealthHerb.Data;
using HealthHerb.Enum;
using HealthHerb.Help;
using HealthHerb.Interface;
using HealthHerb.Models;
using HealthHerb.Models.Product;
using HealthHerb.Models.Settings;
using HealthHerb.Models.User;
using HealthHerb.ViewModels.Setting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly UserManager<BaseUser> userManager;
        private readonly AppDbContext context;
        private readonly FileManager fileManager;

        public SettingController
        (
            ICrud<PaymentSetting> paymentManageCrud,
            ICrud<Order> orderCrud,
            ICrud<FrontEndData> frontendDataCrud,
            ICrud<OrderProduct> orderProductCrud,
            ICrud<ShippingPrice> shippingPriceCrud,
            UserManager<BaseUser> userManager,
            AppDbContext context,
            FileManager fileManager

        )
        {
            this.paymentManageCrud = paymentManageCrud;
            this.orderCrud = orderCrud;
            this.frontendDataCrud = frontendDataCrud;
            this.orderProductCrud = orderProductCrud;
            this.shippingPriceCrud = shippingPriceCrud;
            this.userManager = userManager;
            this.context = context;
            this.fileManager = fileManager;
        }

        public async Task<IActionResult> Index()
        {
            var result = await orderCrud.GetAll(new string[] { "OrderProducts" });

            var totalResult = result.Select(m => m.OrderProducts.Select(x => x.TotalPrice).Sum()).Sum();
            TempData["total"] = totalResult;

            var today = DateTime.Now.AddDays(-1);
            var todayResult = result.Where(m => m.CreatedAt>=today)
                             .Select(m => m.OrderProducts
                            .Select(x => x.TotalPrice).Sum()).Sum();
            TempData["today"] = totalResult;

            var lastWeek = DateTime.Now.AddDays(-7);
            var monthResult = result.Where(m =>m.CreatedAt>=lastWeek)
                                .Select(m => m.OrderProducts
                                .Select(x => x.TotalPrice).Sum()).Sum();
            TempData["week"] = monthResult;

            var lastMonth = DateTime.Now.AddMonths(-1);
            var yearResult = result.Where(m => m.CreatedAt >= lastMonth)
                               .Select(m => m.OrderProducts
                               .Select(x => x.TotalPrice).Sum()).Sum();
            TempData["month"] = yearResult;

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
            payment.SecretKey = model.SecretKey.Trim();
            payment.PublishKey = model.PublishKey.Trim();
            await paymentManageCrud.Update(payment);

            TempData["Success"] = "Success update";
            return Redirect("/setting/index");
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
                BigTitle = frontEndData.BigTitle,
                CurrentImage =frontEndData.Image,
                HeaderText = frontEndData.Header,
                Text = frontEndData.Text
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
            record.BigTitle = model.BigTitle;

            if (model.Image == null)
            {
                record.Image = model.CurrentImage;
            }
            else
            {
                record.Image = fileManager.Upload(model.Image);

                if(model.CurrentImage!=null&& model.Image!=null)
                    fileManager.Delete(model.CurrentImage);
            }
            await frontendDataCrud.Update(record);
            TempData["Success"] = "Success update";
            return Redirect("/setting/index");
        }
    }
}
