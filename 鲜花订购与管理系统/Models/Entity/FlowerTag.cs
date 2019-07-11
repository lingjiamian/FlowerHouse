using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerHouse.Models.Entity
{
    public class FlowerTag
    {
        public int FlowerTagId { set; get; }

        public int FlowerId { set; get; }

        public int TagId { set; get; }



        public Flower Flower { set; get; }

        public Tag Tag { set; get; }
    }
}
