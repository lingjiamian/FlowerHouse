using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerHouse.Models.Entity
{
    /// <summary>
    /// 标签分类表，比如按用途分类，按花材分类
    /// </summary>
    public class Assortment
    {
        public int AssortmentId { set; get; }

        [MaxLength(50)]
        public string AssortmentName { set; get; }



        public List<Tag> Tags { set; get; }
    }
}
