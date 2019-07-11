using FlowerHouse.Models.Entity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerHouse.Areas.Admin.Data
{
    public static class ExtendUserManager
    {
        public static HouseUser GetByPhone(this UserManager<HouseUser> userManager, FlowerHouseContext flowerHouseContext, string phoneNumber)
        {
            var user = flowerHouseContext.Users.FirstOrDefault(u => u.PhoneNumber == phoneNumber);
            return user;
        }
    }
}
