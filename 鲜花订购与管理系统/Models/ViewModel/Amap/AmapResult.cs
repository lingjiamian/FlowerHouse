using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerHouse.Models.ViewModel.Amap
{
    public class AmapResult
    {
        public string status { set; get; }

        public string info { set; get; }

        public string infocode { set; get; }

        public string count { set; get; }

        public AmapSuggestion suggestion { set; get; }

        public AmapCity[] districts { set; get; }
    }
}
