using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlowerHouse.Models;
using FlowerHouse.Models.Entity;

namespace FlowerHouse.Controllers
{
    public class HomeController : Controller
    {
        private readonly FlowerHouseContext _flowerHouseContext;

        public HomeController(FlowerHouseContext flowerHouseContext)
        {
            _flowerHouseContext = flowerHouseContext;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            //精品推荐的九朵花，按购买数降序取出前九个
            var remcommandFlowers = await _flowerHouseContext.Flowers.Include(f => f.Stock).OrderByDescending(f => f.FlowerBuy).Take(8).ToListAsync();

            _flowerHouseContext.Flowers.Skip((page - 1) * 10).Take(10).ToList();

            return View(remcommandFlowers);
        }

        //public async Task<IActionResult> Test(int page = 1)
        //{
        //    //page参数接受刚才a标签里的page的值

        //    //skip再take
        //    return View(//要返回的数据)
        //}
    }
}