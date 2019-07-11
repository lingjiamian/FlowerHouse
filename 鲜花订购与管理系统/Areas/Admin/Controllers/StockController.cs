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
    public class StockController : Controller
    {
        private readonly FlowerHouseContext flowerHouseContext;
        private readonly UserManager<HouseUser> userManager;

        public StockController(FlowerHouseContext flowerHouseContext, UserManager<HouseUser> userManager)
        {
            this.flowerHouseContext = flowerHouseContext;
            this.userManager = userManager;
        }

        public IActionResult List()
        {
            ViewBag.leftNav = "Stock";
            return View();
        }

        public async Task<IActionResult> GetDataList(int page, int limit)
        {
            int count = await flowerHouseContext.Stocks.CountAsync();
            var dataList = await flowerHouseContext.Stocks.Skip((page - 1) * limit).Take(limit).Include(p => p.Flower).Select(s => new { flowerName = s.Flower.FlowerName, purchasePrice = s.PurchasePrice, sellPrice = s.SellPrice, flowerId = s.FlowerId, stockCount = s.StockCount }).ToListAsync();
            return Json(new { Data = dataList, Count = count, Code = "0" });
        }

        public IActionResult CreateOrUpdate()
        {
            return View();
        }

        //public async Task<IActionResult> PurchaseFlower(int? flowerId, int? purchaseCount)
        //{
        //    if (flowerId.HasValue && purchaseCount.Value >= 0)
        //    {

        //        var flower = await flowerHouseContext.Flowers.FindAsync(flowerId);
        //        flower.FlowerCount += purchaseCount.Value;
        //        Purchase purchaseRecord = new Purchase
        //        {
        //            FlowerId = flowerId.Value,
        //            PurchaseCount = purchaseCount.Value,
        //            PurchasePrice = purchaseCount.Value * flower.FlowerPrice,
        //            PurchaseTime = DateTime.Now
        //        };
        //        await flowerHouseContext.Purchases.AddAsync(purchaseRecord);
        //        await flowerHouseContext.SaveChangesAsync();
        //        return Json(new { status = 1, msg = "进货成功", returnUrl = "/Admin/Purchase/List" });
        //    }
        //    else
        //    {
        //        return Json(new { status = 0, msg = "进货失败" });
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> CreateOrUpdate(Purchase purchase)
        //{
        //    int flowerId = purchase.FlowerId;
        //    var flower = await flowerHouseContext.Flowers.FirstOrDefaultAsync(f => f.FlowerId == flowerId);
        //    if (flower == null)
        //    {
        //        ModelState.AddModelError("", "不存在该花朵");
        //        return View(purchase);
        //    }
        //    purchase.PurchaseTime = DateTime.Now;
        //    purchase.PurchasePrice = purchase.PurchaseCount * flower.FlowerPrice;
        //    flower.FlowerCount += purchase.PurchaseCount;
        //    await flowerHouseContext.Purchases.AddAsync(purchase);
        //    await flowerHouseContext.SaveChangesAsync();
        //    return RedirectToAction("List");
        //}
    }
}