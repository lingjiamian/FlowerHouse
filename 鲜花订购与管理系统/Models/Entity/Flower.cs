using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerHouse.Models.Entity
{
    public class Flower
    {
        [Key]
        public int FlowerId { set; get; }

        [MaxLength(100)]
        public string FlowerName { set; get; }

        /// <summary>
        /// 花材
        /// </summary>
        [MaxLength(500)]
        public string FlowerMaterial { set; get; }

        /// <summary>
        /// 花语
        /// </summary>
        public string FlowerEmblem { set; get; }

        /// <summary>
        /// 派送范围，一般都是全国
        /// </summary>
        public string DistributionRange { set; get; }

        public int FlowerBuy { set; get; }

        //public int FlowerPrice { set; get; }

        public string FlowerImg { set; get; }

        //public int FlowerCount { set; get; }

        public Stock Stock { set; get; }

        public List<FlowerTag> FlowerTags { set; get; }

    }
}
