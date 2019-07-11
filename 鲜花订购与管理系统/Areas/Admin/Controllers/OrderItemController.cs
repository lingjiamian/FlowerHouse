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
    public class OrderItemController : Controller
    {
        private readonly FlowerHouseContext flowerHouseContext;
        private readonly UserManager<HouseUser> userManager;

        public OrderItemController(FlowerHouseContext flowerHouseContext, UserManager<HouseUser> userManager)
        {
            this.flowerHouseContext = flowerHouseContext;
            this.userManager = userManager;
        }

        public IActionResult List(int? orderid)
        {
            ViewBag.leftNav = "OrderItem";

            if (orderid.HasValue)
            {
                HttpContext.Session.SetString("orderId", orderid.Value.ToString());
            }
            return View();
        }

        public async Task<IActionResult> CreateOrUpdate(string orderItemId)
        {
            if (orderItemId != null)
            {
                var orderitem = await flowerHouseContext.OrderItems.FindAsync(Convert.ToInt32(orderItemId));
                return View(orderitem);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate(OrderItem orderItem)
        {
            //flowerHouseContext.Entry(flower).State = (flowerHouseContext.Flowers.Any(f => f.FlowerId == flower.FlowerId) ? EntityState.Modified : EntityState.Added);
            var updateEntity = await flowerHouseContext.OrderItems.FirstOrDefaultAsync(o => o.OrderItemId == orderItem.OrderItemId);
            if (updateEntity == null)
            {
                updateEntity = orderItem;
                await flowerHouseContext.AddAsync(updateEntity);
            }
            else
            {
                updateEntity.Count = orderItem.Count;
                updateEntity.FlowerId = orderItem.FlowerId;
                updateEntity.OrderId = orderItem.OrderId;
                flowerHouseContext.Update(updateEntity);
            }
            await flowerHouseContext.SaveChangesAsync();
            return RedirectToAction("List");
        }

        public async Task<IActionResult> GetDataList(int page, int limit)
        {
            int count = await flowerHouseContext.OrderItems.CountAsync();
            var orderItemList = flowerHouseContext.OrderItems.Include(oi => oi.Order).Include(o => o.Flower).ThenInclude(f => f.FlowerTags).ThenInclude(ft => ft.Tag).Skip((page - 1) * limit).Take(limit).ToList();
            //orderItemId = oi.OrderItemId,
            //count = oi.Count,
            //flowerId = oi.FlowerId,
            //flowerName = oi.Flower.FlowerName,
            //orderId = oi.OrderId,
            //orderTime = oi.Order.CreateTime,
            //flowerTag = oi.Flower.FlowerTags[0].Tag.TagName
            var dataList = orderItemList.Select(oi => new
            {
                orderItemId = oi.OrderItemId,
                orderItemStatus = Enum.GetName(typeof(OrderItemStatus), oi.OrderItemStatus),
                count = oi.Count,
                flowerId = oi.FlowerId,
                flowerName = oi.Flower.FlowerName,
                orderId = oi.OrderId,
                orderTime = oi.Order.CreateTime,
                flowerTag = oi.Flower.FlowerTags[0].Tag.TagName,
            });
            string findOrderId = HttpContext.Session.GetString("orderId");
            if (findOrderId != null)
            {
                dataList = dataList.Where(oi => oi.orderId == Convert.ToInt32(findOrderId)).ToList();
                HttpContext.Session.Remove("orderId");
            }
            return Json(new { Data = dataList, Count = count, Code = "0" });
        }

        public async Task<IActionResult> DeleteData(int orderItemId)
        {
            var deleteEntity = await flowerHouseContext.OrderItems.FirstOrDefaultAsync(f => f.FlowerId == orderItemId);

            if (deleteEntity != null)
            {
                flowerHouseContext.OrderItems.Remove(deleteEntity);
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