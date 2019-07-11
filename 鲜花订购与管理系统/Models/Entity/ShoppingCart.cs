using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerHouse.Models.Entity
{
    public class ShoppingCart
    {
        public int ShoppingCartId { set; get; }

        public string HouseUserId { set; get; }

        public int TotalPrice { set; get; }



        public HouseUser HouseUser { set; get; }

        public List<CartItem> CartItems { set; get; }
    }
}
