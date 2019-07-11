using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerHouse.Models.Entity
{
    public class HouseUser : IdentityUser
    {
        public int Balance { set; get; }
        public ShoppingCart ShoppingCart { set; get; }
        public List<Order> Orders { set; get; }
    }
}
