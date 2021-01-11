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
    [Authorize(Policy ="ManageHouse")]
    public class FlowerController : Controller
    {
        private readonly FlowerHouseContext flowerHouseContext;
        private readonly UserManager<HouseUser> userManager;

        public FlowerController(FlowerHouseContext flowerHouseContext, UserManager<HouseUser> userManager)
        {
            this.flowerHouseContext = flowerHouseContext;
            this.userManager = userManager;
        }

        public IActionResult List()
        {
            ViewBag.leftNav = "Flower";
            return View();
        }

        public async Task<IActionResult> CreateOrUpdate(string flowerid)
        {
            ViewBag.TagList = await flowerHouseContext.Tags.ToListAsync();
            if (flowerid != null)
            {
                var flower = await flowerHouseContext.Flowers.Include(f => f.FlowerTags).FirstOrDefaultAsync(f => f.FlowerId == Convert.ToInt32(flowerid));
                return View(flower);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate(Flower flower, int[] tagid)
        {
            var updateFlower = await flowerHouseContext.Flowers.Include(f => f.Stock).FirstOrDefaultAsync(f => f.FlowerId == flower.FlowerId);
            if (updateFlower == null)
            {
                updateFlower = flower;
                updateFlower.FlowerBuy = -1;
                //Purchase purchase = new Purchase
                //{
                //    Flower = updateFlower,
                //    PurchaseCount = updateFlower.FlowerCount,
                //    PurchasePrice = updateFlower.FlowerCount * updateFlower.FlowerPrice,
                //    PurchaseTime = DateTime.Now
                //};
                //await flowerHouseContext.AddAsync(purchase);
                await flowerHouseContext.AddAsync(updateFlower);
            }
            else
            {
                updateFlower.FlowerName = flower.FlowerName;
                updateFlower.FlowerMaterial = flower.FlowerMaterial;
                updateFlower.FlowerEmblem = flower.FlowerEmblem;
                updateFlower.DistributionRange = flower.DistributionRange;
                updateFlower.FlowerBuy = flower.FlowerBuy;
                //updateFlower.FlowerPrice = flower.FlowerPrice;
                updateFlower.FlowerImg = flower.FlowerImg;

                flowerHouseContext.Update(updateFlower);
            }
            updateFlower.FlowerTags = new List<FlowerTag>();
            foreach (int tid in tagid)
            {
                updateFlower.FlowerTags.Add(new FlowerTag { FlowerId = updateFlower.FlowerId, TagId = tid });
            }
            await flowerHouseContext.SaveChangesAsync();
            return RedirectToAction("List");
        }

        public async Task<IActionResult> GetFlowerData(int page, int limit)
        {
            int count = await flowerHouseContext.Flowers.CountAsync();
            var dataList = await flowerHouseContext.Flowers.Skip((page - 1) * limit).Take(limit).ToListAsync();
            return Json(new LayTableResult<Flower> { Data = dataList, Count = count, Code = "0" });
        }

        public async Task<IActionResult> DeleteData(int flowerid)
        {
            var deleteEntity = await flowerHouseContext.Flowers.FirstOrDefaultAsync(f => f.FlowerId == flowerid);
            if (deleteEntity != null)
            {
                flowerHouseContext.Flowers.Remove(deleteEntity);
                await flowerHouseContext.SaveChangesAsync();
                return Json(new { status = 1, msg = "删除成功" });
            }
            else
            {
                return Json(new { status = 0, msg = "删除失败" });
            }
        }

        //上传图片接口
        public async Task<IActionResult> UploadImage([FromServices] IWebHostEnvironment hostingEnvironment, IFormFile file)
        {
            long size = file.Length;

            string filePath = hostingEnvironment.WebRootPath + $@"\images\flowerImg\";

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            int code = 0;

            string fileFullName = filePath + file.FileName;

            string src = @"/images/flowerImg/" + file.FileName;

            await Task.Run(() =>
            {
                using (FileStream fs = new FileStream(fileFullName, FileMode.Create))
                {
                    try
                    {
                        file.CopyTo(fs);
                    }
                    catch
                    {
                        code = 1;
                    }
                }
            });

            string msg = "上传成功";

            object resultJson = new { code, msg, src };
            return Json(resultJson);
        }
    }
}