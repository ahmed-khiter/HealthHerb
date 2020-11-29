using HealthHerb.Authorization;
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
    [Authorize(Roles = Role.Admin)]
    public class DiscountController : Controller
    {
        private readonly ICrud<Coupon> crud;

        public DiscountController
        (
            ICrud<Coupon> crud
        )
        {
            this.crud = crud;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await crud.GetAll());
        }
        
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Coupon model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await crud.Add(model);

            ViewData["Success"] = "Success create";
            return RedirectToAction(nameof(Create));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var model = await crud.GetById(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Coupon model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await crud.Update(model);

            ViewData["Success"] = "Success create";

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

            ViewData["Success"] = "Success delete";

            return RedirectToAction(nameof(Index));
        }
    }
}
