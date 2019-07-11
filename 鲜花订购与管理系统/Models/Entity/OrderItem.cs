using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerHouse.Models.Entity
{
    public class OrderItem
    {
        public int OrderItemId { set; get; }

        public int Count { set; get; }

        public int FlowerId { set; get; }

        public int OrderId { set; get; }

        public OrderItemStatus OrderItemStatus { set; get; }

        public Flower Flower { set; get; }

        public Order Order { set; get; }
    }

    public enum OrderItemStatus
    {
        待支付,
        已支付,
        待发货,
        已发货,
        已收货,
        已关闭
    }
}
