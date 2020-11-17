using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthHerb.Authorization;
using HealthHerb.Help;
using HealthHerb.Interface;
using HealthHerb.Models.Product;
using HealthHerb.ViewModels.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthHerb.Controllers
{
    //[Authorize(Roles =Role.Admin)]
    [AllowAnonymous]
    public class ProductController : Controller
    {
        private readonly FileManager fileManager;
        private readonly ICrud<Product> crud;

        public ProductController
        (
            FileManager fileManager,
            ICrud<Product> crud
        )
        {
            this.fileManager = fileManager;
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
            return View(new ProductViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Create));
            }

            var record = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Discount = model.Discount,
                Quantity = model.Quantity, 
                Image = fileManager.Upload(model.Image),
            };

            await crud.Add(record);

            TempData["Success"] = "Success operation";

            return RedirectToAction(nameof(Create));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string Id)
        {
            var record = await crud.GetById(Id);

            var model = new ProductViewModel()
            {
                Id = record.Id,
                Name = record.Name,
                Description = record.Description,
                Price = record.Price,
                Discount = record.Discount,
                Quantity = record.Quantity,
                CurrentImage = record.Image
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var record = await crud.GetById(model.Id);

            record.Name = model.Name;
            record.Description = model.Description;
            record.Price = model.Price;
            record.Discount = model.Discount;
            record.Quantity = model.Quantity;

            if (model.Image == null)
            {
                record.Image = model.CurrentImage;
            }
            else
            {
                record.Image = fileManager.Upload(model.Image);
            }

            await crud.Update(record);

            TempData["Success"] = "Success operation";

            return RedirectToAction(nameof(Edit), new { id = model.Id });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            await crud.Delete(id);

            return Ok();
        }
    }
}
