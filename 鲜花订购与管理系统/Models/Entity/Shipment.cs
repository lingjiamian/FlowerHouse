using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerHouse.Models.Entity
{
    public class Shipment
    {
        public int ShipmentId { set; get; }

        public int ShipmentCount { set; get; }

        public DateTime ShipmentTime { set; get; }

        public int FlowerId { set; get; }

        public int OrderItemId { set; get; }

        public int ShipmentPrice { set; get; }

        public Flower Flower { set; get; }

        public OrderItem OrderItem { set; get; }
    }
}
