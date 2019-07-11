using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerHouse.Models.Entity
{
    public class Purchase
    {
        public int PurchaseId { set; get; }

        public DateTime PurchaseTime { set; get; }

        public int PurchaseCount { set; get; }

        public int FlowerId { set; get; }

        public int PurchasePrice { set; get; }

        public Flower Flower { set; get; }
    }
}
