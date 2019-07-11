using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerHouse.Models.JsonEntity.Amap
{
    /// <summary>
    /// 高德地图api结果
    /// </summary>
    public class AmapJsonResult
    {
        public string Status { set; get; }
        public string Info { set; get; }
        public string InfoCode { set; get; }
        public string Count { set; get; }
    }
}
