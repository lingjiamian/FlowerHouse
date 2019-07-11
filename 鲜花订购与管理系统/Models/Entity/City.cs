using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerHouse.Models.Entity
{
    public class City
    {
        [Key]
        [MaxLength(50)]
        public string Id { set; get; }

        [MaxLength(100)]
        public string Name { set; get; }

        [MaxLength(50)]
        public string ProvinceId { set; get; }

        public List<Area> ChildList { set; get; }
    }
}
