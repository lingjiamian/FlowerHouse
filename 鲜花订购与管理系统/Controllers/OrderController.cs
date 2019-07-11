using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FlowerHouse.Models.Entity;
using FlowerHouse.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace FlowerHouse.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly FlowerHouseContext _flowerHouseContext;
        private readonly UserManager<HouseUser> _userManager;

        public OrderController(UserManager<HouseUser> userManager, FlowerHouseContext flowerHouseContext)
        {
            _userManager = userManager;
            _flowerHouseContext = flowerHouseContext;
        }

        public async Task<IActionResult> Buy()
        {
            var shoppingCart = await GetShoppingCartAsync(await GetCurrentUser());
            return View(shoppingCart);
        }

        /// <summary>
        /// 生成订单
        /// </summary>
        /// <param name="address">订单地址</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> BuyConfirmed(string address)
        {
            var currentUser = await GetCurrentUser();
            var shoppingCart = await GetShoppingCartAsync(currentUser);

            if (shoppingCart == null)
            {
                return Json(new { status = 0, msg = "购物车为空" });
            }

            Order order = new Order
            {
                Address = address,
                CreateTime = DateTime.Now,
                TimeOutTime = DateTime.Now.AddMinutes(30),
                HouseUser = currentUser,
                OrderStatus = OrderStatus.待支付,
                OrderItems = new List<OrderItem>()
            };

            foreach (var checkedCartItem in shoppingCart.CartItems.Where(ci => ci.IsChecked))
            {
                order.OrderItems.Add(new OrderItem
                {
                    Count = checkedCartItem.Count,
                    FlowerId = checkedCartItem.FlowerId
                });
                checkedCartItem.Flower.Stock.StockCount -= checkedCartItem.Count;
                order.TotalPrice += checkedCartItem.Flower.Stock.SellPrice * checkedCartItem.Count;
                shoppingCart.TotalPrice -= checkedCartItem.Flower.Stock.SellPrice * checkedCartItem.Count;
            }

            shoppingCart.CartItems.RemoveAll(ci => ci.IsChecked);

            await _flowerHouseContext.Orders.AddAsync(order);

            await _flowerHouseContext.SaveChangesAsync();

            string returnUrl = "/Order/Pay?orderid=" + order.OrderId;

            return Json(new { status = 1, returnUrl });
        }

        public async Task<IActionResult> Pay(string orderid)
        {
            var order = await _flowerHouseContext.Orders.Include(o => o.HouseUser).FirstOrDefaultAsync(o => o.OrderId == Convert.ToInt32(orderid));
            return View(order);
        }

        public async Task<IActionResult> PayConfirmed(string orderid)
        {
            var order = await _flowerHouseContext.Orders.Include(o => o.HouseUser).Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.OrderId == Convert.ToInt32(orderid));
            var currentUser = await GetCurrentUser();


            if (order.HouseUserId == currentUser.Id && order.OrderStatus == OrderStatus.待支付)
            {
                if (currentUser.Balance >= order.TotalPrice)
                {
                    currentUser.Balance -= order.TotalPrice;
                    order.OrderStatus = OrderStatus.已支付;
                    order.OrderItems.ForEach(oi =>
                    {
                        oi.OrderItemStatus = OrderItemStatus.已支付;
                    });
                    order.FinishTime = DateTime.Now;
                    await _flowerHouseContext.SaveChangesAsync();
                    return Json(new { status = 1, msg = "支付成功", returnUrl = "/Order/Detail?orderid=" + orderid });
                }
                else
                {
                    return Json(new { status = 0, msg = "余额不足" });
                }
            }

            else
            {
                return Json(new { status = 0, msg = "支付失败" });
            }
        }

        public async Task<IActionResult> ReceiveConfirmed(int? orderid)
        {
            var order = await _flowerHouseContext.Orders.Include(o => o.HouseUser).FirstOrDefaultAsync(o => o.OrderId == Convert.ToInt32(orderid));
            var user = await GetCurrentUser();
            if (order.HouseUserId == user.Id)
            {
                order.OrderStatus = OrderStatus.已收货;
                await _flowerHouseContext.SaveChangesAsync();
                return Json(new { status = 1, msg = "收货成功", returnUrl = "/Order/List" });
            }
            else
            {
                return Json(new { status = 0, msg = "收货失败" });
            }

        }
        public async Task<IActionResult> List()
        {
            var currentUser = await GetCurrentUser();
            var orderList = await _flowerHouseContext.Orders.Include(o => o.HouseUser).Where(o => o.HouseUserId == currentUser.Id).OrderByDescending(o => o.CreateTime).ToListAsync();
            return View(orderList);
        }

        public async Task<IActionResult> Detail(string orderid)
        {
            var order = await GetOrder(orderid);
            return View(order);
        }

        private async Task<Order> GetOrder(string orderid)
        {
            var currentUser = await GetCurrentUser();
            var order = await _flowerHouseContext.Orders.Include(o => o.HouseUser).FirstOrDefaultAsync(o => o.OrderId == Convert.ToInt32(orderid));
            return order;
        }

        private async Task<HouseUser> GetCurrentUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                return await _userManager.GetUserAsync(principal: User);
            }
            else
            {
                return null;
            }
        }

        private async Task<ShoppingCart> GetShoppingCartAsync(HouseUser currentUser)
        {
            return await _flowerHouseContext.ShoppingCarts.Include(sc => sc.CartItems).ThenInclude(ci => ci.Flower).ThenInclude(f => f.Stock).FirstOrDefaultAsync(sc => sc.HouseUserId == currentUser.Id);
        }
    }
}