using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerHouse.Models.Entity
{
    public class Stock
    {
        public int StockId { set; get; }

        public int StockCount { set; get; }

        public int PurchasePrice { set; get; }

        public int SellPrice { set; get; }

        public int FlowerId { set; get; }

        public Flower Flower { set; get; }
    }
}
