using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerHouse.Models.ViewModel.Amap
{
    public class AmapCity
    {
        public string[] citycode { set; get; }

        public string center { set; get; }

        public string level { set; get; }

        public string adcode { set; get; }

        public string name { set; get; }

        public AmapCity[] districts { set; get; }
    }
}
