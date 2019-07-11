using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerHouse.Areas.Admin.Models
{
    public class LayTableResult<T>
    {
        public string Code { set; get; }

        public string Message { set; get; }

        public int Count { set; get; }

        public List<T> Data { set; get; }
    }
}
