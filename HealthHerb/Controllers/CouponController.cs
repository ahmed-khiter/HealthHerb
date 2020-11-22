using HealthHerb.Authorization;
using HealthHerb.Interface;
using HealthHerb.Models.Product;
using HealthHerb.Models.User;
using HealthHerb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthHerb.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICrud<CouponUsed> couponUsedCrud;
        private readonly ICrud<Coupon> crud;
        private readonly UserManager<BaseUser> userManager;

        public CouponController
        (
            ICrud<CouponUsed> couponUsedCrud,
            ICrud<Coupon> crud,
            UserManager<BaseUser> userManager
        )
        {
            this.couponUsedCrud = couponUsedCrud;
            this.crud = crud;
            this.userManager = userManager;
        }

        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var model = await crud.GetAll();
            return View(model);
        }

        [Authorize(Roles = Role.Admin)]
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

        [Authorize(Roles = Role.Admin)]
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
        [Authorize(Roles = Role.Admin)]
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
        [Authorize(Roles = Role.Admin)]
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
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> Delete(string id)
        {
            await crud.Delete(id);

            return RedirectToAction(nameof(List));
        }

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsCodeInUse(string Code)
        {
            var user = await crud.GetFirst(m => m.Code.Equals(Code));
            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"This code : {Code} is already in use ");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CheckCoupon(PrepareProductViewModel model)
        {
            var record = await crud.GetById(m => m.Code.Equals(model.DiscountCode));
            var userId = userManager.GetUserId(User);

            if (record == null)
            {
                return NotFound();
            }

            if (record.ManyUsed > 0 && (DateTime.Now > record.Start || DateTime.Now < record.End))
            {
                var copupon = await couponUsedCrud.GetFirst(m => m.CouponId.Equals(record.Id) && m.BaseUserId.Equals(userId));
                if (copupon == null)
                {
                    record.ManyUsed -= 1;
                    await crud.Update(record);
                    await couponUsedCrud.Add(new CouponUsed
                    {
                        BaseUserId = userId,
                        CouponId = record.Id
                    });
                    model.TotalPrice = model.TotalPrice - (model.TotalPrice*(decimal)(record.Amount/100));
                    return Ok(new { totalPrice = model.TotalPrice });
                }
                return BadRequest();
               
            }
            else
            {
                return NotFound();
            }
        }

    }
}
