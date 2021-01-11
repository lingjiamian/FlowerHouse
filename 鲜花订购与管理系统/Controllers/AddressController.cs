using FlowerHouse.Models.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace 鲜花订购与管理系统.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly FlowerHouseContext _flowerHouseContext;
        public AddressController(FlowerHouseContext flowerHouseContext)
        {
            _flowerHouseContext = flowerHouseContext;
        }

        public async Task<IActionResult> GetShengshi()
        {
            var shengshi = await _flowerHouseContext.Provinces.AsNoTracking().Include(p => p.ChildList).ThenInclude(c => c.ChildList).ToListAsync();
            var jsonresult = new JsonResult(shengshi);
            return jsonresult;
        }

    }
}
