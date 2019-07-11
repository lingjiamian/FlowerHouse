using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlowerHouse.Areas.Admin.Models;
using FlowerHouse.Models.Entity;
using FlowerHouse.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowerHouse.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PurchaseController : Controller
    {
        private readonly FlowerHouseContext flowerHouseContext;
        private readonly UserManager<HouseUser> userManager;

        public PurchaseController(FlowerHouseContext flowerHouseContext, UserManager<HouseUser> userManager)
        {
            this.flowerHouseContext = flowerHouseContext;
            this.userManager = userManager;
        }

        public IActionResult List()
        {
            ViewBag.leftNav = "Purchase";
            return View();
        }

        public async Task<IActionResult> GetDataList(int page, int limit)
        {
            int count = await flowerHouseContext.Purchases.CountAsync();
            var dataList = await flowerHouseContext.Purchases.Skip((page - 1) * limit).Take(limit).Include(p => p.Flower).Select(p => new { flowerName = p.Flower.FlowerName, purchaseCount = p.PurchaseCount, purchaseTime = p.PurchaseTime, flowerId = p.FlowerId, purchasePrice = p.PurchasePrice }).ToListAsync();
            return Json(new { Data = dataList, Count = count, Code = "0" });
        }

        public IActionResult PurchaseOldFlower()
        {
            return View();
        }

        public IActionResult PurchaseNewFlower()
        {
            return View();
        }

        /// <summary>
        /// 获取标签和花的API
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetTagFlowerList(bool getNewToPurchase)
        {
            //List<Tag> tagList = new List<Tag>();
            var tagList = await flowerHouseContext.Tags.Include(t => t.FlowerTags).ThenInclude(ft => ft.Flower).ToListAsync();

            if (getNewToPurchase)
            {
                //tagList = tagList.Where(t => t.FlowerTags.Where(ft => ft.Flower.FlowerBuy < 0).Count() != 0).ToList();
                tagList.ForEach(t =>
                {
                    t.FlowerTags = t.FlowerTags.Where(ft => ft.Flower.FlowerBuy < 0).ToList();
                });
                tagList = tagList.Where(t => t.FlowerTags.Count != 0).ToList();
            }

            var dataList = new List<LaySelectN>();

            tagList.ForEach(t =>
            {
                LaySelectN laySelectN = new LaySelectN
                {
                    ChildList = new List<LaySelectN>(),
                    Id = t.TagId,
                    Name = t.TagName
                };
                t.FlowerTags.ForEach(ft =>
                {
                    laySelectN.ChildList.Add(new LaySelectN
                    {
                        Id = ft.FlowerId,
                        Name = ft.Flower.FlowerName
                    });
                });
                dataList.Add(laySelectN);
            });
            return Json(dataList);
        }

        public async Task<IActionResult> PurchaseFlower(int? flowerId, int? purchaseCount, int? sellPrice, int? purchasePrice)
        {
            if (flowerId.HasValue && purchaseCount.Value >= 0)
            {
                var flower = await flowerHouseContext.Flowers.Include(f => f.Stock).FirstOrDefaultAsync(f => f.FlowerId == flowerId.Value);
                if (sellPrice.HasValue && flower.Stock == null && purchaseCount.HasValue)
                {
                    Stock stock = new Stock
                    {
                        FlowerId = flowerId.Value,
                        PurchasePrice = purchasePrice.Value,
                        SellPrice = sellPrice.Value,
                        StockCount = purchaseCount.Value

                    };
                    flower.Stock = stock;
                    flower.FlowerBuy = 0;
                    await flowerHouseContext.Stocks.AddAsync(stock);
                }
                else
                {
                    flower.Stock.StockCount += purchaseCount.Value;

                }
                Purchase purchaseRecord = new Purchase
                {
                    FlowerId = flowerId.Value,
                    PurchaseCount = purchaseCount.Value,
                    PurchasePrice = purchaseCount.Value * flower.Stock.PurchasePrice,
                    PurchaseTime = DateTime.Now
                };
                await flowerHouseContext.Purchases.AddAsync(purchaseRecord);
                await flowerHouseContext.SaveChangesAsync();
                return Json(new { status = 1, msg = "进货成功", returnUrl = "/Admin/Purchase/List" });
            }
            else
            {
                return Json(new { status = 0, msg = "进货失败" });
            }
        }

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