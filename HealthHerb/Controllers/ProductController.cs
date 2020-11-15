using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthHerb.Help;
using HealthHerb.Interface;
using HealthHerb.Models.Product;
using HealthHerb.ViewModels.Products;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthHerb.Controllers
{
    public class ProductController : Controller
    {
        private readonly FileManager fileManager;
        private readonly ICrud<Product> crud;
        private readonly ICrud<Image> imageCrud;

        public ProductController
        (
            FileManager fileManager,
            ICrud<Product> crud,
            ICrud<Image> imageCrud
        )
        {
            this.fileManager = fileManager;
            this.crud = crud;
            this.imageCrud = imageCrud;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var model = await crud.GetAll(new string[] { "Images" });
            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                RedirectToAction(nameof(Create));
            }

            var record = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Discount = model.Discount,
                Quantity = model.Quantity, 
            };

            await crud.Add(record);

            await CreateImage(record.Id, model.Images);

            TempData["Success"] = "Success operation";

            return RedirectToAction(nameof(Create));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string Id)
        {
            var record = await crud.GetById(Id);
            var images = await imageCrud.GetAll(img => img.ProductId.Equals(Id));

            var model = new ProductViewModel()
            {
                Id = record.Id,
                Name = record.Name,
                Description = record.Description,
                Price = record.Price,
                Discount = record.Discount,
                Quantity = record.Quantity
            };

            foreach (var image in images)
            {
                model.CurrentImages.Add(new ImageViewModel
                {
                    ImageId = image.Id,
                    CurrentImage = image.Name,
                });
            }

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

            await crud.Update(record);

            await UpdateImage(model.CurrentImages, record.Id);

            TempData["Success"] = "Success operation";

            return RedirectToAction(nameof(Edit), new { id = model.Id });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            await DeleteImage(id);
            await crud.Delete(id);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeletePhoto(string id)
        {
            await imageCrud.Delete(id);
            return Ok();
        }

        private async Task UpdateImage(List<ImageViewModel> images, string productId)
        {
            foreach (var image in images)
            {
                if (image.ImageId == null)
                {
                    if (image.NewImage != null)
                    {
                        var newRecord = new Image
                        {
                            Name = fileManager.Upload(image.NewImage),
                            ProductId = productId
                        };

                        await imageCrud.Add(newRecord);
                    }

                    continue;
                }

                var record = await imageCrud.GetById(image.ImageId);

                if (image.NewImage != null)
                {
                    fileManager.Delete(record.Name);
                    record.Name = fileManager.Upload(image.NewImage);
                    await imageCrud.Update(record);
                }
            }
        }

        private async Task DeleteImage(string productId)
        {
            var images = await imageCrud.GetAll(img => img.ProductId.Equals(productId));

            foreach (var image in images)
            {
                fileManager.Delete(image.Name);
            }

            await imageCrud.Delete(img => img.ProductId.Equals(productId));
        }

        private async Task CreateImage(string productId, List<IFormFile> images)
        {
            if (productId == null)
            {
                return;
            }

            string image = "noimage.jpg";

            if (images == null)
            {
                await imageCrud.Add(new Image
                {
                    Name = image,
                    ProductId = productId,
                });
            }

            for (int i = 0; i < images.Count; i++)
            {
                await imageCrud.Add(new Image
                {
                    Name = fileManager.Upload(images[i]),
                    ProductId = productId,
                });
            }
        }
    }
}
