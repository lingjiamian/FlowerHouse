using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FlowerHouse.Models.Entity;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using Newtonsoft.Json;
using FlowerHouse.Models.ViewModel.Amap;
using Newtonsoft.Json.Linq;

namespace FlowerHouse.Api
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

        //public async Task<IActionResult> AddAddress(string addressee, string addressDetails, string phone)
        //{
        //    Address address = new Address
        //    {
        //        AddressDetails = addressDetails,
        //        phone = phone,
        //        Addressee = addressee
        //    };
        //    return Ok();
        //}

        public async Task<IActionResult> GetShengshi()
        {
            var shengshi = await _flowerHouseContext.Provinces.AsNoTracking().Include(p => p.ChildList).ThenInclude(c => c.ChildList).ToListAsync();
            var jsonresult = new JsonResult(shengshi);
            return jsonresult;
        }

        //public async Task<IActionResult> GetShengshi()
        //{
        //    var client = new RestClient("https://restapi.amap.com/v3/config/district?key=9be91ac44711341369ca529124bcba6e&subdistrict=2");
        //    var request = new RestRequest(Method.GET);
        //    var response =  await client.ExecuteTaskAsync(request);

        //    JObject amapResult = JObject.Parse(response.Content);

        //    var lai = amapResult["districts"];
        //    var arr = JArray.Parse(lai.ToString());
        //    //var boy = lai["districts"].ToString();
        //    //JObject country = (JObject)(amapResult["districts"].ToString());
        //    var boy = arr.Last.Last;
        //    JsonResult jsonResult = new JsonResult(arr[3]);

        //    //JObject provinces = (JObject)country["districts"];

        //    return new JsonResult(lai);
        //    //var shengshi = await _flowerHouseContext.Provinces.AsNoTracking().Include(p => p.ChildList).ThenInclude(c => c.ChildList).ToListAsync();
        //    //var jsonresult = new JsonResult(shengshi);
        //    //return jsonresult;
        //}
    }
}