using FlowerHouse.Models;
using FlowerHouse.Models.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerHouse.Component
{
    /// <summary>
    /// 轮播图组件
    /// </summary>
    public class CarouselViewComponent:ViewComponent
    {
        private readonly FlowerHouseContext _flowerHouseContext;

        public CarouselViewComponent(FlowerHouseContext flowerHouseContext)
        {
            _flowerHouseContext = flowerHouseContext;
            

        }

        public IViewComponentResult Invoke()
        {

            return View();
        }
    }
}
