using System;
using System.Collections.Generic;
using System.Text;

namespace EgtDemo.Common.Helper
{
    /// <summary>
    /// 缓存特性：作用于方法
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class CachingAttribute : Attribute
    {
        /// <summary>
        /// 过期时间
        /// </summary>
        public int AbsoluteExpiration { get; set; } = 30;
    }
}
