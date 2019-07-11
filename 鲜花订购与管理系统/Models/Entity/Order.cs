using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerHouse.Models.Entity
{
    public class Order
    {
        [Key]
        public int OrderId { set; get; }

        public DateTime? CreateTime { set; get; }

        public DateTime? TimeOutTime { set; get; }

        public DateTime? FinishTime { set; get; }

        public int TotalPrice { set; get; }

        //是否已支付
        //public bool IsPayed { set; get; }

        public string Address { set; get; }

        public string HouseUserId { set; get; }

        public OrderStatus OrderStatus
        {
            set; get;
        }

        public List<OrderItem> OrderItems { set; get; }

        public HouseUser HouseUser { set; get; }
    }

    public enum OrderStatus
    {
        待支付,
        已支付,
        待发货,
        已发货,
        已收货,
        已关闭
    }
}
