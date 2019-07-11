using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlowerHouse.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowerHouse.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ShipmentController : Controller
    {
        private readonly FlowerHouseContext flowerHouseContext;
        private readonly UserManager<HouseUser> userManager;

        public ShipmentController(FlowerHouseContext flowerHouseContext, UserManager<HouseUser> userManager)
        {
            this.flowerHouseContext = flowerHouseContext;
            this.userManager = userManager;
        }

        public IActionResult List()
        {
            ViewBag.leftNav = "Shipment";

            return View();
        }

        public async Task<IActionResult> GetDataList(int page, int limit)
        {
            int count = await flowerHouseContext.Shipments.CountAsync();
            var dataList = await flowerHouseContext.Shipments.Skip((page - 1) * limit).Take(limit).Include(s => s.Flower).Include(s => s.OrderItemId).Select(s => new { flowerName = s.Flower.FlowerName, flowerId = s.FlowerId, shipmentTime = s.ShipmentTime, shipmentCount = s.ShipmentCount, shipmentPrice = s.ShipmentPrice, orderItemId = s.OrderItemId }).ToListAsync();
            return Json(new { Data = dataList, Count = count, Code = "0" });
        }

        public async Task<IActionResult> GetNotShipmentOrderItemList()
        {
            var orderItemList = await flowerHouseContext.OrderItems.Where(oi => oi.OrderItemStatus == OrderItemStatus.待发货).ToListAsync();
            return Json(orderItemList);
        }

        public IActionResult CreateOrUpdate()
        {
            return View();
        }

        public async Task<IActionResult> ShipmentFlower(int? flowerId, int? orderItemId, int? shipmentCount)
        {
            if (flowerId.HasValue && orderItemId.HasValue && shipmentCount.HasValue)
            {
                var flower = await flowerHouseContext.Flowers.Include(f => f.Stock).FirstOrDefaultAsync(f => f.FlowerId == flowerId.Value);
                var orderItem = await flowerHouseContext.OrderItems.Include(oi => oi.Order).ThenInclude(o => o.OrderItems).FirstOrDefaultAsync(oi => oi.OrderItemId == orderItemId.Value);
                if (flower != null && orderItem != null && orderItem.FlowerId == flowerId.Value)
                {

                    orderItem.OrderItemStatus = OrderItemStatus.已发货;
                    if (orderItem.Order.OrderItems.All(oi => oi.OrderItemStatus == OrderItemStatus.已发货))
                    {
                        orderItem.Order.OrderStatus = OrderStatus.已发货;
                    }
                    Shipment shipment = new Shipment
                    {
                        FlowerId = flowerId.Value,
                        OrderItemId = orderItemId.Value,
                        ShipmentCount = shipmentCount.Value,
                        ShipmentPrice = shipmentCount.Value * flower.Stock.SellPrice,
                        ShipmentTime = DateTime.Now
                    };
                    await flowerHouseContext.Shipments.AddAsync(shipment);
                    await flowerHouseContext.SaveChangesAsync();
                    return Json(new { status = 1, msg = "出货成功", returnUrl = "/Admin/Shipment/List" });
                }
            }
            return Json(new { status = 0, msg = "出货失败" });
        }


       

        //[HttpPost]
        //public async Task<IActionResult> CreateOrUpdate(Shipment shipment)
        //{
        //    int flowerId = shipment.FlowerId;
        //    var flower = await flowerHouseContext.Flowers.Include(f => f.Stock).FirstOrDefaultAsync(f => f.FlowerId == flowerId);
        //    if (flower == null)
        //    {
        //        ModelState.AddModelError("", "不存在该花朵");
        //        return View(shipment);
        //    }
        //    var orderItem = flowerHouseContext.OrderItems.Include(oi => oi.Order).ThenInclude(o => o.OrderItems).FirstOrDefault(oi => oi.OrderId == shipment.OrderItemId);
        //    if (orderItem != null && orderItem.OrderItemStatus == OrderItemStatus.待发货)
        //    {
        //        flower.Stock.StockCount -= shipment.ShipmentCount;
        //        shipment.ShipmentTime = DateTime.Now;
        //        shipment.ShipmentPrice = shipment.ShipmentCount * flower.Stock.SellPrice;
        //        orderItem.OrderItemStatus = OrderItemStatus.已发货;
        //        if (orderItem.Order.OrderItems.All(oi => oi.OrderItemStatus == OrderItemStatus.已发货))
        //        {
        //            orderItem.Order.OrderStatus = OrderStatus.已发货;
        //        }
        //        await flowerHouseContext.Shipments.AddAsync(shipment);
        //        await flowerHouseContext.SaveChangesAsync();
        //        return RedirectToAction("List");
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "不存在该订单详情");
        //        return View(shipment);
        //    }
        //}
    }
}