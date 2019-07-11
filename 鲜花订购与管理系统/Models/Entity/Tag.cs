using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerHouse.Models.Entity
{
    public class Tag
    {
        public int TagId { set; get; }

        public string TagName { set; get; }

        public string TagImg { set; get; }

        public int AssortmentId { set; get; }



        public Assortment Assortment { set; get; }

        public List<FlowerTag> FlowerTags { set; get; }
    }
}
