using System;
using System.Collections.Generic;
using System.Text;

namespace EgtDemo.Common.Helper
{
    public interface ICaching
    {
        object Get(string cacheKey);
        void Set(string cacheKey, object cacheValue);
    }
}
