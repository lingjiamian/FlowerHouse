using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerHouse.Models.JsonResult
{
    //试用
    public class JsonResponse
    {
        public int status { set; get; }
        public string msg { set; get; }
    }
    public enum Status
    {
        Fail,
        Success
    }
}
