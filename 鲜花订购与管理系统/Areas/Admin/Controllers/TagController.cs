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
    public class TagController : Controller
    {
        private readonly FlowerHouseContext flowerHouseContext;
        private readonly UserManager<HouseUser> userManager;

        public TagController(FlowerHouseContext flowerHouseContext, UserManager<HouseUser> userManager)
        {
            this.flowerHouseContext = flowerHouseContext;
            this.userManager = userManager;
        }

        public IActionResult List()
        {
            ViewBag.leftNav = "Tag";

            return View();
        }

        public async Task<IActionResult> CreateOrUpdate(string tagid)
        {
            if (tagid != null)
            {
                var tag = await flowerHouseContext.Tags.FindAsync(Convert.ToInt32(tagid));
                return View(tag);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate(Tag tag)
        {
            var updateTag = await flowerHouseContext.Tags.FirstOrDefaultAsync(t => t.TagId == tag.TagId);
            if (updateTag == null)
            {
                await flowerHouseContext.AddAsync(tag);
            }
            else
            {
                updateTag.TagName = tag.TagName;
                updateTag.TagImg = tag.TagImg;
                updateTag.AssortmentId = tag.AssortmentId;
            }
            await flowerHouseContext.SaveChangesAsync();
            return RedirectToAction("List");
        }

        /// <summary>
        /// 表格数据接口  
        /// </summary>
        /// <param name="page">页数</param>
        /// <param name="limit">每页数据数目</param>
        /// <returns></returns>
        public async Task<IActionResult> GetDataList(int page, int limit)
        {
            int count = await flowerHouseContext.Tags.CountAsync();
            var dataList = await flowerHouseContext.Tags.Skip((page - 1) * limit).Take(limit).ToListAsync();
            return Json(new LayTableResult<Tag> { Data = dataList, Count = count, Code = "0" });
        }

        public async Task<IActionResult> DeleteData(int tagid)
        {
            var deleteEntity = await flowerHouseContext.Tags.FirstOrDefaultAsync(t => t.TagId == tagid);

            if (deleteEntity != null)
            {

                flowerHouseContext.Tags.Remove(deleteEntity);

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

            string filePath = hostingEnvironment.WebRootPath + $@"\images\icons\";

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            int code = 0;
            string fileFullName = filePath + file.FileName;
            string src = @"/images/icons/" + file.FileName;
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