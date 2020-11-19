using HealthHerb.Data;
using HealthHerb.Interface;
using HealthHerb.Models.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthHerb.Controllers
{
    public class OrderController : Controller
    {
        private readonly ICrud<Order> crud;
        private readonly AppDbContext context;

        public OrderController
        (
            ICrud<Order> crud,
            AppDbContext context
        )
        {
            this.crud = crud;
            this.context = context;
        }
        public async Task<IActionResult> Index()
        {
            var record = await context.Orders
                        .Include(m => m.OrderProducts).ThenInclude(m => m.BaseUser)
                        .Include(m=>m.OrderProducts).ThenInclude(m=>m.Product)
                        .ToListAsync();
            return View(record);
        }
    }
}
