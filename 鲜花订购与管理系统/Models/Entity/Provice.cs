﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerHouse.Models.Entity
{
    public class Province
    {
        [Key]
        [MaxLength(50)]
        public string Id { set; get; }

        [MaxLength(100)]
        public string Name { set; get; }

        public List<City> ChildList { set; get; }
    }
}
