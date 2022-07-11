using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace EgtDemo.Common.AOP
{
    public class BlogLogAOP : IInterceptor
    {
        private readonly ILogger<BlogLogAOP> _logger;
        public BlogLogAOP(ILogger<BlogLogAOP> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// 动态拦截器
        /// </summary>
        /// <param name="invocation">被调用的方法（当前执行的方法，即环绕它作切面编程）</param>
        public void Intercept(IInvocation invocation)
        {
            //方法前：
            var dataIntercept = $"【当前方法】：{invocation.Method.Name}\r\n" +
                              $"【携带参数】：{string.Join(",", invocation.Arguments.Select(a => (a ?? "").ToString()))}";

            //继续执行当前方法
            invocation.Proceed();

            //方法后：
            dataIntercept += $"【执行结果】：{invocation.ReturnValue}";

            Parallel.For(0, 1, e =>
            {
                _logger.Log(LogLevel.Information, $"BlogLogAop:{dataIntercept}");
            });
        }
    }
}
