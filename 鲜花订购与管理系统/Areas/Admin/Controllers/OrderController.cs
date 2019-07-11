using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlowerHouse.Areas.Admin.Models;
using FlowerHouse.Models.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowerHouse.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "ManageHouse")]
    public class OrderController : Controller
    {
        private readonly FlowerHouseContext flowerHouseContext;
        private readonly UserManager<HouseUser> userManager;

        public OrderController(FlowerHouseContext flowerHouseContext, UserManager<HouseUser> userManager)
        {
            this.flowerHouseContext = flowerHouseContext;
            this.userManager = userManager;
        }

        public IActionResult List()
        {
            ViewBag.leftNav = "Order";
            return View();
        }

        public async Task<IActionResult> CreateOrUpdate(string orderid)
        {
            if (orderid != null)
            {
                var order = await flowerHouseContext.Orders.FindAsync(Convert.ToInt32(orderid));
                return View(order);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AcceptOrder(string orderId)
        {
            var order = await flowerHouseContext.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.OrderId == Convert.ToInt32(orderId));
            if (order.OrderStatus == OrderStatus.已支付)
            {
                order.OrderStatus = OrderStatus.待发货;
                order.OrderItems.ForEach(oi =>
                {
                    oi.OrderItemStatus = OrderItemStatus.待发货;
                });

                order.FinishTime = DateTime.Now;
                await flowerHouseContext.SaveChangesAsync();
                return Json(new { status = 1, msg = "接单成功" });
            }
            else
            {
                return Json(new { status = 0, msg = "接单失败" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RefuseOrder(string orderId)
        {
            var order = await flowerHouseContext.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Flower).ThenInclude(f => f.Stock).FirstOrDefaultAsync(o => o.OrderId == Convert.ToInt32(orderId));
            if (order.OrderStatus == OrderStatus.已支付 || order.OrderStatus == OrderStatus.待支付)
            {
                order.OrderStatus = OrderStatus.已关闭;
                order.OrderItems.ForEach(oi =>
                {
                    oi.Flower.Stock.StockCount += oi.Count;
                });
                order.OrderItems.ForEach(oi =>
                {
                    oi.OrderItemStatus = OrderItemStatus.已关闭;
                });
                order.FinishTime = DateTime.Now;
                await flowerHouseContext.SaveChangesAsync();
                return Json(new { status = 1, msg = "拒单成功" });
            }
            else
            {
                return Json(new { status = 0, msg = "拒单失败" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate(Order order)
        {
            //flowerHouseContext.Entry(flower).State = (flowerHouseContext.Flowers.Any(f => f.FlowerId == flower.FlowerId) ? EntityState.Modified : EntityState.Added);
            var updateEntity = await flowerHouseContext.Orders.FirstOrDefaultAsync(o => o.OrderId == order.OrderId);
            if (updateEntity == null)
            {
                await flowerHouseContext.AddAsync(order);
            }
            else
            {
                updateEntity.CreateTime = order.CreateTime;
                updateEntity.TimeOutTime = order.CreateTime.Value.AddMinutes(30);
                updateEntity.FinishTime = order.FinishTime;
                updateEntity.TotalPrice = order.TotalPrice;
                updateEntity.Address = order.Address;
                updateEntity.HouseUserId = order.HouseUserId;
                updateEntity.OrderStatus = order.OrderStatus;
                flowerHouseContext.Update(updateEntity);
            }
            await flowerHouseContext.SaveChangesAsync();
            return RedirectToAction("List");
        }

        public async Task<IActionResult> GetDataList(int page, int limit)
        {
            int count = await flowerHouseContext.Orders.CountAsync();
            var dataList = await flowerHouseContext.Orders.Skip((page - 1) * limit).Take(limit).Select(o => new { orderId = o.OrderId, createTime = o.CreateTime, finishTime = o.FinishTime, timeOutTime = o.TimeOutTime, totalPrice = o.TotalPrice, address = o.Address, houseUserId = o.HouseUserId, orderStatus = Enum.GetName(typeof(OrderStatus), o.OrderStatus) }).ToListAsync();
            return Json(new { Data = dataList, Count = count, Code = "0" });
        }

        public async Task<IActionResult> DeleteData(int orderid)
        {
            var deleteEntity = await flowerHouseContext.Orders.FirstOrDefaultAsync(o => o.OrderId == orderid);

            if (deleteEntity != null)
            {
                flowerHouseContext.Orders.Remove(deleteEntity);
                await flowerHouseContext.SaveChangesAsync();
                return Json(new { status = 1, msg = "删除成功" });
            }
            else
            {
                return Json(new { status = 0, msg = "删除失败" });
            }
        }
    }
}