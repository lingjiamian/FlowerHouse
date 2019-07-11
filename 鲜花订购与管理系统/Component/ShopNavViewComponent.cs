using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlowerHouse.Models;
using FlowerHouse.Models.Entity;

namespace FlowerHouse.Component
{
        /// <summary>
        /// 商品导购组件
        /// </summary>
    public class ShopNavViewComponent : ViewComponent
    {
        private readonly FlowerHouseContext _flowerHouseContext;

        public ShopNavViewComponent(FlowerHouseContext flowerHouseContext)
        {
            _flowerHouseContext = flowerHouseContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //获取分类，用Include把该分类的Tag获取了
            var ass = await _flowerHouseContext.Assortments.Include(a => a.Tags).ToListAsync();
            return View(ass);
        }
    }
}
