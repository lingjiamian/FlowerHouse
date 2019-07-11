using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FlowerHouse.Controllers
{
    /// <summary>
    /// 搜索控制器
    /// </summary>
    public class SearchController : Controller
    {
        public IActionResult Keyword(string[] goodID)
        {
            //重定向回Flower控制器
            return View();
        }
    }
}