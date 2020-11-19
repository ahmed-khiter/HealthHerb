using HealthHerb.Interface;
using HealthHerb.Models.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthHerb.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICrud<Coupon> crud;

        public CouponController
        (
            ICrud<Coupon> crud
        )
        {
            this.crud = crud;
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var model = await crud.GetAll();
            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new Coupon
            {
                Start = DateTime.Now,
                End = DateTime.Now.AddDays(1),
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Coupon model)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Create));
            }

            var record = new Coupon
            {
               Amount = model.Amount,
               Code = model.Code,
               Start = model.Start,
               End = model.End,
               ManyUsed = model.ManyUsed,
            };

            await crud.Add(record);

            TempData["Success"] = "Success operation";

            return RedirectToAction(nameof(Create));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string Id)
        {
            var record = await crud.GetById(Id);

            var model = new Coupon()
            {
                Id = record.Id,
                Amount = record.Amount,
                Code = record.Code,
                Start = record.Start,
                End = record.End,
                ManyUsed = record.ManyUsed,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Coupon model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var record = await crud.GetById(model.Id);

            if (record == null)
            {
                return RedirectToAction(nameof(List));
            }

            record.Amount = model.Amount;
            record.Code = model.Code;
            record.Start = model.Start;
            record.End = model.End;
            record.ManyUsed = model.ManyUsed;

            await crud.Update(record);

            TempData["Success"] = "Success operation";

            return RedirectToAction(nameof(Edit), new { id = model.Id });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            await crud.Delete(id);

            return RedirectToAction(nameof(List));
        }

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsCodeInUse(string Code)
        {
            var User = await crud.GetAll(m => m.Code.Equals(Code));
            if (User == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"This code : {Code} is already in use ");
            }
        }
    }
}
