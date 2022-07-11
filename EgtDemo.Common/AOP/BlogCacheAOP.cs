using Castle.DynamicProxy;
using EgtDemo.Common.Helper;
using System.Linq;

namespace EgtDemo.Common.AOP
{
    public class BlogCacheAOP : AOPbase
    {
        private readonly ICaching _cache;

        public BlogCacheAOP(ICaching cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// 缓存拦截器---如果被调用方法的返回结果已存在缓存，则直接取，不用去数据库拿，否则拿到数据要存入缓存
        /// </summary>
        /// <param name="invocation"></param>
        public override void Intercept(IInvocation invocation)
        {
            var method = invocation.MethodInvocationTarget ?? invocation.Method;
            var attr = method.GetCustomAttributes(true).FirstOrDefault(o => o.GetType() == typeof(CachingAttribute)) as CachingAttribute;
            //只有指定特性的方法才被缓存
            if (attr != null)
            {
                //拿到缓存key
                var cacheKey = base.CustomCacheKey(invocation);
                var cacheValue = _cache.Get(cacheKey);
                if (cacheValue != null)
                {
                    //说明已经缓存过，直接从缓存中取即可
                    invocation.ReturnValue = cacheValue;
                    return;
                }

                //执行当前被调用的方法
                invocation.Proceed();

                if (!string.IsNullOrWhiteSpace(cacheKey))
                {
                    _cache.Set(cacheKey, invocation.ReturnValue);
                }
            }
        }
    }
}
