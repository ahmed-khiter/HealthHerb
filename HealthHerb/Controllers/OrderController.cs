using HealthHerb.Authorization;
using HealthHerb.Data;
using HealthHerb.Enum;
using HealthHerb.Interface;
using HealthHerb.Models.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthHerb.Controllers
{
    [Authorize(Roles = Role.Admin)]
    public class OrderController : Controller
    {
        private readonly ICrud<Order> crud;
        private readonly ICrud<OrderProduct> orderProductCrud;
        private readonly AppDbContext context;

        public OrderController
        (
            ICrud<Order> crud,
            ICrud<OrderProduct> orderProductCrud,
            AppDbContext context
        )
        {
            this.crud = crud;
            this.orderProductCrud = orderProductCrud;
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var record = await context.Orders
                        .Include(m => m.OrderProducts).ThenInclude(m => m.BaseUser)
                        .Include(m=>m.OrderProducts).ThenInclude(m=>m.Product)
                        .ToListAsync();
           

            return View(record);
        }

        [HttpGet]
        public async Task<IActionResult> SpecificOrder(string id)
        {
            var model = await context.Orders
                                .Include(m => m.OrderProducts)
                                .ThenInclude(m => m.Product)
                                .SingleOrDefaultAsync(m => m.Id.Equals(id));
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditOrderStatus(string orderId, OrderStatus orderStatus)
        {
            var order = await crud.GetById(orderId);
            order.OrderStatus = orderStatus;
            await crud.Update(order);
            TempData["Success"] = "Operation success";
            return RedirectToAction("Index");
        }
    }
}
