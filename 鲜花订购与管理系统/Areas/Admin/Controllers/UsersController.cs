using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FlowerHouse.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FlowerHouse.Areas.Admin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace FlowerHouse.Areas.Admin.Controllers
{
    /// <summary>
    /// 暂时弃用
    /// </summary>
    [Area("Admin")]
    [Authorize(Policy = "ManageHouse")]
    public class UsersController : Controller
    {
        private readonly FlowerHouseContext flowerHouseContext;
        private readonly UserManager<HouseUser> userManager;

        public UsersController(FlowerHouseContext flowerHouseContext, UserManager<HouseUser> userManager)
        {
            this.flowerHouseContext = flowerHouseContext;
            this.userManager = userManager;
        }

        public IActionResult List()
        {
            ViewBag.leftNav = "Users";
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string username, string password, string phonenumber)
        {
            HouseUser newUser = new HouseUser
            {
                Id = phonenumber,
                UserName = username
            };
            await userManager.CreateAsync(newUser, password);
            return Json(new { status = 1, msg = "创建成功", returnUrl = "/Admin/Users/List" });
        }

        public async Task<IActionResult> CreateOrUpdate(string userid)
        {
            if (userid != null)
            {

                HouseUser user = await flowerHouseContext.Users.FindAsync(userid);
                return View(user);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate(HouseUser houseUser)
        {
            var user = await userManager.FindByIdAsync(houseUser.Id);
            if (user == null)
            {
                await flowerHouseContext.Users.AddAsync(houseUser);
            }

            else
            {
                user.Balance = houseUser.Balance;
                user.Email = houseUser.Email;
                user.EmailConfirmed = houseUser.EmailConfirmed;
                user.UserName = houseUser.UserName;
            }

            await flowerHouseContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> GetUesrs(int page, int limit)
        {
            var dataList = await flowerHouseContext.Users.ToListAsync();
            int count = dataList.Count;
            return Json(new LayTableResult<HouseUser> { Data = dataList.Skip((page - 1) * limit).Take(limit).ToList(), Count = count, Code = "0" });
        }
    }
}