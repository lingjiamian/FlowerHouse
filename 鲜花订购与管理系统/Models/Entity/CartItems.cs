using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerHouse.Models.Entity
{
    public class CartItem
    {
        public int CartItemId { set; get; }

        public int FlowerId { set; get; }

        public int Count { set; get; }

        public int ShoppingCartId { set; get; }

        //有没有被勾选
        public bool IsChecked { set; get; }

        public ShoppingCart ShoppingCart { set; get; }

        public Flower Flower { set; get; }
    }
}
