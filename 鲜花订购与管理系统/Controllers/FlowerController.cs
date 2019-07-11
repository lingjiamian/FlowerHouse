using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FlowerHouse.Models;
using FlowerHouse.Models.Entity;
using Microsoft.AspNetCore.Identity;

namespace FlowerHouse.Controllers
{
    public class FlowerController : Controller
    {
        private readonly FlowerHouseContext _context;

        public FlowerController(FlowerHouseContext context)
        {
            _context = context;
        }

        //private async Task<List<Flower>> GetFlowerList()
        //{
        //    var flowerList = await _context.Flowers.Include(f => f.Stock).Where(f => f.FlowerBuy >= 0).ToListAsync();
        //    return flowerList;
        //}

        //点击查看全部时列出所有花
        public async Task<IActionResult> List()
        {
            return View(await _context.Flowers.Include(f => f.Stock).Where(f => f.FlowerBuy >= 0).ToListAsync());
        }

        //鲜花详情页
        public async Task<IActionResult> Detail(int? id, [FromServices]UserManager<HouseUser> userManager)
        {
            string userId;
            if (User.Identity.IsAuthenticated)
            {
                userId = userManager.GetUserId(User);
            }
            if (id == null)
            {
                return NotFound();
            }
            var flower = await _context.Flowers.AsNoTracking().Include(f => f.Stock).Include(f => f.FlowerTags).ThenInclude(ft => ft.Tag)
                .FirstOrDefaultAsync(m => m.FlowerId == id);
            if (flower == null)
            {
                return NotFound();
            }
            return View(flower);
        }

        //标签
        public async Task<IActionResult> Tag(int? Id)
        {
            //取出该标签的所有花
            var flowerlist = await _context.Flowers.AsNoTracking().Include(f => f.Stock).Include(f => f.FlowerTags).Where(f => f.FlowerTags.Any(ft => ft.TagId == Id)).Where(f => f.FlowerBuy>=0).ToListAsync();

            //面包屑
            ViewBag.breadcrumb = (await _context.Tags.AsNoTracking().FirstOrDefaultAsync(t => t.TagId == Id)).TagName;

            return View("List", flowerlist);
        }

        //搜索
        public async Task<IActionResult> Search(string inputframe)
        {
            //查询 花名，花材，花语 是否包含keyword
            var flowerList = await _context.Flowers.Include(f => f.Stock).Where(f => (f.FlowerName.Contains(inputframe) || f.FlowerMaterial.Contains(inputframe) || f.FlowerEmblem.Contains(inputframe)) && f.FlowerBuy >=0).ToListAsync();

            //var flowerList = (await GetFlowerList()).Where(f => f.FlowerName.Contains(inputframe) || f.FlowerMaterial.Contains(inputframe) || f.FlowerEmblem.Contains(inputframe)).ToList();
            //查询 标签 是否包含keyword
            var tagList = await _context.Tags.AsNoTracking().Where(t => t.TagName.Contains(inputframe)).Include(t => t.FlowerTags).ThenInclude(ft => ft.Flower).ThenInclude(f => f.Stock).ToListAsync();

            if (tagList != null)
            {
                foreach (var tag in tagList)
                {
                    foreach (var ft in tag.FlowerTags)
                    {
                        flowerList.Add(ft.Flower);
                    }
                }
            }
            flowerList = flowerList.Where(f => f.FlowerBuy >= 0).ToList();
            if (flowerList.Count == 0)
            {
                ViewBag.tip = "无搜索结果";
            }
            //面包屑
            ViewBag.breadcrumb = inputframe;

            return View("List", flowerList);
        }
    }
}
