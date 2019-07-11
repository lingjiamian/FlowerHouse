using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlowerHouse.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FlowerHouse.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace FlowerHouse.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly UserManager<HouseUser> _userManager;
        private readonly FlowerHouseContext _flowerHouseContext;
        public ShoppingCartController(UserManager<HouseUser> userManager, FlowerHouseContext flowerHouseContext)
        {
            _flowerHouseContext = flowerHouseContext;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> CartDetail()
        {
            var shoppingCart = await GetShoppingCartAsync(await GetCurrentUser());
            return View(shoppingCart);
        }

        //添加进购物车，点击加入购物车或者点击
        [HttpPost]
        public async Task<IActionResult> AddToCart(int? flowerId)
        {
            HouseUser currnetUser = await GetCurrentUser();
            if (!flowerId.HasValue)
            {
                return Json(new { status = 0, msg = "添加失败" });
            }

            if (currnetUser == null)
            {
                return Json(new { status = 0, msg = "请先登录" });
            }

            var flowerAddToCart = await _flowerHouseContext.Flowers.Include(f => f.Stock).FirstOrDefaultAsync(f => f.FlowerId == flowerId.Value);
            var shoppingCart = await GetShoppingCartAsync(currnetUser);
            if (shoppingCart == null)
            {
                shoppingCart = new ShoppingCart
                {
                    HouseUserId = currnetUser.Id,
                    TotalPrice = 0,
                    CartItems = new List<CartItem>()
                };
                await _flowerHouseContext.AddAsync(shoppingCart);
            }
            var exitedCartItem = shoppingCart.CartItems.FirstOrDefault(ci => ci.FlowerId == flowerId.Value);
            if (exitedCartItem == null)
            {
                exitedCartItem = new CartItem
                {
                    Count = 1,
                    IsChecked = true,
                    Flower = flowerAddToCart
                };
                shoppingCart.CartItems.Add(exitedCartItem);
            }
            else
            {
                exitedCartItem.Count += 1;
                exitedCartItem.IsChecked = true;
            }
            ComputeTotalPrice(shoppingCart);
            await _flowerHouseContext.SaveChangesAsync();
            return Json(new { status = 1, msg = "添加成功" });
        }

        //购物车打勾
        [Authorize]
        public async Task<IActionResult> CheckToCart(int? cartItemId, int? cartId, bool check)
        {
            var shoppingCart = await GetShoppingCartAsync(await GetCurrentUser());
            if (cartId.HasValue)
            {
                if (shoppingCart.ShoppingCartId == cartId.Value)
                {
                    shoppingCart.CartItems.ForEach(ci => ci.IsChecked = check);
                }
                else
                {
                    return Json(new { status = 0, msg = "操作失败" });
                }
            }
            else
            {
                var cartItem = shoppingCart.CartItems.FirstOrDefault(c => c.CartItemId == cartItemId);
                cartItem.IsChecked = check;
            }
            ComputeTotalPrice(shoppingCart);
            await _flowerHouseContext.SaveChangesAsync();
            bool allCheck = shoppingCart.CartItems.All(ci => ci.IsChecked);
            return Json(new { totalPrice = shoppingCart.TotalPrice});
        }

        //移出购物车
        [HttpPost]
        public async Task<IActionResult> RemoveCartItem(int? cartitemid)
        {
            var shoppingCart = await GetShoppingCartAsync(await GetCurrentUser());
            if (cartitemid.HasValue)
            {
                var removeCartItem = shoppingCart.CartItems.FirstOrDefault(ci => ci.CartItemId == cartitemid.Value);
                if(removeCartItem != null)
                {
                    shoppingCart.CartItems.Remove(removeCartItem);
                }
                ComputeTotalPrice(shoppingCart);
                await _flowerHouseContext.SaveChangesAsync();
                return Json(new { status = 1, totalprice = shoppingCart.TotalPrice,msg="移除成功",cartcount=shoppingCart.CartItems.Count });
            }
            else
            {
                return Json(new { status = 0,msg="移除失败" });
            }

        }

        //计算购物车总价
        private void ComputeTotalPrice( ShoppingCart shoppingCart)
        {
            shoppingCart.TotalPrice = 0;
            foreach(var ci in shoppingCart.CartItems)
            {
                if (ci.IsChecked)
                {
                    shoppingCart.TotalPrice += ci.Count * ci.Flower.Stock.SellPrice;
                }
            }
        }

        //获取当前购物车
        private async Task<ShoppingCart> GetShoppingCartAsync(HouseUser currentUser)
        {
            return await _flowerHouseContext.ShoppingCarts.Include(sc => sc.CartItems).ThenInclude(ci => ci.Flower).ThenInclude(f => f.Stock).FirstOrDefaultAsync(sc => sc.HouseUserId == currentUser.Id);
        }

        //获取当前用户
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
    }
}