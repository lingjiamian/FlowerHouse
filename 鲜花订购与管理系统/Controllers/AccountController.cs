using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using FlowerHouse.Models.JsonResult;
using FlowerHouse.Models.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using FlowerHouse.Utility;
using Newtonsoft.Json;

namespace FlowerHouse.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<HouseUser> _userManager;
        private readonly SignInManager<HouseUser> _signInManager;
        private readonly FlowerHouseContext flowerHouseContext;

        public AccountController(UserManager<HouseUser> userManager, SignInManager<HouseUser> signInManager, FlowerHouseContext flowerHouseContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this.flowerHouseContext = flowerHouseContext;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Login")]
        public async Task<IActionResult> LoginAsync(string loginName, string password, string vcode)
        {
            //TODO:后端验证
            if (vcode == null || vcode.ToLower() != HttpContext.Session.GetString("vcode"))
            {
                return Json(new { status = 0, msg = "验证码不正确" });
            }
            HouseUser user = await _userManager.FindByIdAsync(loginName);
           
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
                if (result.Succeeded)
                {
                    if (user.Id == "15015015011")
                    {   
                        return Json(new { status = 1, msg = "登陆成功", returnUrl = "/Admin/Flower/List" });
                    }
                    return Json(new { status = 1, msg = "登陆成功", returnUrl = "/Home/Index" });
                }
            }
            return Json(new { status = 0, msg = "用户名或密码不正确" });
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View("Login");
        }

        //注册请求手机验证码
        [HttpPost]
        public async Task<IActionResult> RegisterRequestCode(string phone)
        {
            if (!CheckUtility.IsPhone(phone))
            {
                return Json(new JsonResponse { status = 0, msg = "手机号不正确" });
            }
            if (await ExitedPhone(phone))
            {
                //试用一下Json相应模型
                return Json(new JsonResponse { status = 0, msg = "手机号已注册" });
            }

            var response = await RequestCodeAsync(phone);

            if (response.IsSuccessful)
            {
                return Json(new JsonResponse { status = 1, msg = "验证码已发送" });
            }
            else
            {
                return Json(new JsonResponse { status = 0, msg = "验证码发送失败" });
            }
        }

        [HttpPost]
        [ActionName("Register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegiserAsync(string phone, string userName, string password, string validate)
        {   
            //后端验证
            if (!CheckUtility.IsUserName(userName))
            {
                return Json(new { status = 0, msg = "昵称不合法" });
            }

            if (!CheckUtility.IsPhone(phone))
            {
                return Json(new { status = 0, msg = "手机号不合法" });
            }

            //验证手机验证码
            var response = await VerifyCodeAsync(phone, validate);

            if (response.IsSuccessful)
            {
                HouseUser newUser = new HouseUser
                {
                    Id = phone,
                    UserName = userName,
                    PhoneNumber = phone,
                    PhoneNumberConfirmed = true
                };
                await _userManager.CreateAsync(newUser, password);
                await _signInManager.SignInAsync(newUser, false);
                return Json(new { status = 1, msg = "注册成功", returnUrl = "/Home/Index" });
            }
            return Json(new { status = 0, msg = "验证手机失败" });
        }


        #region 忘记密码

        /// <summary>
        /// 忘记密码第一页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult LostFirst()
        {
            return View("Lost", "_firstLostAsync");
        }

        /// <summary>
        /// 忘记密码第一页面请求手机验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> LostRequestCode(string phone)
        {
            var exitedPhone = await ExitedPhone(phone);
            if (!CheckUtility.IsPhone(phone))
            {
                return Json(new { status = 0, msg = "手机号不合法" });
            }
            if (!exitedPhone)
            {
                return Json(new { status = 0, msg = "用户不存在" });
            }
            var response = await RequestCodeAsync(phone);
            if (response.IsSuccessful)
            {
                return Json(new { status = 1, msg = "验证码发送成功" });
            }
            else
            {
                return Json(new { status = 0, msg = "发送失败" });
            }
        }

        /// <summary>
        /// 忘记密码第一页面提交
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <param name="Verification">手机验证码</param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("LostFirst")]
        public async Task<IActionResult> LostFirstAsync(string phone, string Verification)
        {
            if (!CheckUtility.IsPhone(phone))
            {
                return Json(new { status = 0, msg = "手机号不合法" });
            }

            if (!CheckUtility.IsPhoneCode(Verification))
            {
                return Json(new { status = 0, msg = "验证码不合法" });
            }
            var user = await _userManager.FindByIdAsync(phone);

            //用户存在
            if (user != null)
            {
                var response = await VerifyCodeAsync(phone, Verification);
                if (response.IsSuccessful)
                {
                    //修改密码的方法有两个，一个是_userManager.ChangePassword,一个是_userManager.ResetPassword，这里采用第二个
                    //生成一个token用于修改密码
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    //把token和手机号都存进session
                    HttpContext.Session.SetString("phone", phone);
                    HttpContext.Session.SetString("resetPasswordToken", token);

                    //return Redirect("/Account/Lost");
                    //return View("LostSecond");

                    return Json(new { status = 1, returnUrl = "/Account/LostSecond" });
                }
                else
                {
                    return Json(new { status = 0, msg = "修改失败" });
                }
            }
            else
            {
                return Json(new { status = 0, msg = "用户不存在" });
            }
        }

        [HttpGet]
        public IActionResult LostSecond()
        {
            string resetPasswordToken = HttpContext.Session.GetString("resetPasswordToken");
            //如果token为空则重定向到首页
            if (resetPasswordToken == null)
            {
                return Redirect("/Home/Index");
            }
            return View("Lost", "_secondLostAsync");
        }

        /// <summary>
        /// 忘记密码第二页面提交
        /// </summary>
        /// <param name="password">修改的密码</param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("LostSecond")]
        public async Task<IActionResult> LostSecondAsync(string password)
        {
            string resetPasswordToken = HttpContext.Session.GetString("resetPasswordToken");
            //如果token为空则重定向到首页
            if (resetPasswordToken == null)
            {
                return Redirect("/Home/Index");
            }
            var phone = HttpContext.Session.GetString("phone");
            var user = await _userManager.FindByIdAsync(phone);
            var result = await _userManager.ResetPasswordAsync(user, resetPasswordToken, password);
            if (result.Succeeded)
            {
                return Json(new { status = 1, returnUrl = "/Account/LostThird" });
            }
            return Json(new { status = 0, msg = "修改失败" });
        }

        /// <summary>
        /// 忘记密码第三页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult LostThird()
        {
            string resetPasswordToken = HttpContext.Session.GetString("resetPasswordToken");
            if (resetPasswordToken == null)
            {
                return Redirect("/Home/Index");
            }
            HttpContext.Session.Remove("resetPasswordToken");
            HttpContext.Session.Remove("phone");
            return View("Lost", "_thirdLostAsync");
        }



        #endregion

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/Home/Index");
        }

        #region 图形验证码
        private string Vcode(int count)
        {
            Random r = new Random();
            string code = string.Empty;
            string str = "0123456789abcdefghijkmnpqrstuvwxyzABCDEFGHIJKLMNPQRSTUVWXYZ";
            for (int i = 0; i < count; i++)
            {
                int index = r.Next(str.Length);
                code += str[index];
            }
            return code;
        }

        public IActionResult GetVcode()
        {
            string vcode = Vcode(4);
            int width = vcode.Length * 20;
            byte[] bytes;
            using (Image img = new Bitmap(width, 40))
            {
                using (Graphics g = Graphics.FromImage(img))
                {
                    g.Clear(Color.White);
                    Font f = new Font("楷体", 20, FontStyle.Italic);
                    g.DrawString(vcode, f, Brushes.Red, 8, 10);
                }
                MemoryStream stream = new MemoryStream();
                img.Save(stream, ImageFormat.Jpeg);
                bytes = stream.ToArray();
            }
            HttpContext.Session.SetString("vcode", vcode.ToLower());
            return File(bytes, @"image/jpeg");
        }

        #endregion

        #region 手机验证码
        /// <summary>
        /// 发送验证码请求，（请求手机验证码或验证手机验证码）
        /// </summary>
        /// <param name="url">请求的url</param>
        /// <param name="phone">手机号</param>
        /// <returns></returns>
        private async Task<RestResponse> SendRequest(string url, string phone)
        {
            var client = new RestClient(url);
            var request = new RestRequest(url, Method.Post);
            request.AddHeader("X-Bmob-Application-Id", "1b7b23055b070b5bc0badfb56165409c");
            request.AddHeader("X-Bmob-REST-API-Key", "2cfcc9e12f7d0f77b458ee2ddc018452");
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new { mobilePhoneNumber = phone });
            return await client.ExecuteAsync(request);
        }

        
        /// <summary>
        /// 请求手机验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        private async Task<RestResponse> RequestCodeAsync(string phone)
        {
            RestResponse response = await SendRequest("https://api.bmob.cn/1/requestSmsCode", phone);
            return response;
        }

        /// <summary>
        /// 验证手机验证码
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <param name="validate">验证码</param>
        /// <returns></returns>
        private async Task<RestResponse> VerifyCodeAsync(string phone, string validate)
        {
            string url = "https://api.bmob.cn/1/verifySmsCode/" + validate;
            RestResponse response = await SendRequest(url, phone);
            return response;
        }

        #endregion

        /// <summary>
        /// 检查用户名唯一性
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        private async Task<IActionResult> ExitedUserName(string username)
        {
            var result = await _userManager.FindByNameAsync(username);
            return Json(!(result.UserName == username));
        }

        /// <summary>
        /// 检查手机号唯一性
        /// </summary>
        /// <param name="mobilePhoneNumber"></param>
        /// <returns></returns>
        private async Task<bool> ExitedPhone(string mobilePhoneNumber)
        {
            var exited = await _userManager.Users.AnyAsync(u => u.PhoneNumber == mobilePhoneNumber);
            return (exited);
        }

        [HttpGet]
        public IActionResult UserInformation()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> payment(int num)
        {
            
            HouseUser currentUser =await _userManager.GetUserAsync(User);
            currentUser.Balance = currentUser.Balance + num;
            await flowerHouseContext.SaveChangesAsync();
            return Redirect("/Home/Index");
            //return Json(new { msg = "充值成功" });
        }
    }
}