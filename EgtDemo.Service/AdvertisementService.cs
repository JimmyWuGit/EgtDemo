using EgtDemo.Common.Helper;
using EgtDemo.IService;
using EgtDemo.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace EgtDemo.Service
{
    public class AdvertisementService : IAdvertisementService
    {
        public int Test() => 1;

        [Caching(AbsoluteExpiration = 60)]
        public List<Advertisement> TestAOP() => new List<Advertisement>() { new Advertisement { ID = 007, Name = "DreamyZhao" } };
    }
}
