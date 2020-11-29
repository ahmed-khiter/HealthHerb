using HealthHerb.Authorization;
using HealthHerb.Interface;
using HealthHerb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthHerb.Controllers
{
    [Authorize(Roles = Role.Admin)]
    public class ShippingPriceController : Controller
    {
        private readonly ICrud<ShippingPrice> crud;

        public ShippingPriceController
        (
            ICrud<ShippingPrice> crud
        )
        {
            this.crud = crud;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var result = await crud.GetAll();
            result.OrderBy(a => a.Country);
            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ShippingPrice model)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Create));
            }

            var record = new ShippingPrice
            {
                Country = model.Country,
                Price = model.Price,
            };

            await crud.Add(record);

            ViewData["Success"] = "Success operation";

            return RedirectToAction(nameof(Create));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string Id)
        {
            var record = await crud.GetById(Id);

            var model = new ShippingPrice()
            {
                Id = record.Id,
                Country = record.Country,
                Price = record.Price,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ShippingPrice model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var record = await crud.GetById(model.Id);

            record.Country = model.Country;
            record.Price = model.Price;

            await crud.Update(record);

            
            ViewData["Success"] = "Success operation";

            return RedirectToAction(nameof(Edit), new { id = model.Id });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            await crud.Delete(id);

            ViewData["Success"] = "Success Delete";
            return RedirectToAction(nameof(List));
        }

    }
}
