using HealthHerb.Interface;
using HealthHerb.Models.Product;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthHerb.Controllers
{
    public class DiscountController : Controller
    {
        private readonly ICrud<Discount> crud;

        public DiscountController
        (
            ICrud<Discount> crud
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
        public async Task<IActionResult> Create(Discount model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await crud.Add(model);

            TempData["success"] = "Success create";
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
        public async Task<IActionResult> Edit(Discount model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await crud.Update(model);

            TempData["success"] = "Success create";

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

            TempData["success"] = "Success create";

            return RedirectToAction(nameof(Index));
        }
    }
}
