using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerHouse.Models.ViewModel
{
    public class LaySelectN
    {
        public int Id { set; get; }

        public string Name { set; get; }

        public List<LaySelectN> ChildList { set; get; }
    }
}
