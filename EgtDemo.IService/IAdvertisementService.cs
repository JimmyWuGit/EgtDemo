using EgtDemo.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace EgtDemo.IService
{
    public interface IAdvertisementService
    {
        int Test();
        List<Advertisement> TestAOP();
    }
}
