using Microsoft.Extensions.DependencyInjection;
using StackExchange.Profiling.Storage;
using System;

namespace EgtDemo.Extensions
{
    public static class MiniProfilerSetup
    {
        public static void AddMiniProfilerSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            //3.x使用MiniProfiler 必须要注册MemoryCache服务！！！
            services.AddMiniProfiler(o =>
            {
                o.RouteBasePath = "/profiler"; //监测信息的看板地址
                //(o.Storage as MemoryCacheStorage).CacheDuration = TimeSpan.FromMinutes(10);
                o.PopupRenderPosition = StackExchange.Profiling.RenderPosition.Left;
                o.PopupShowTimeWithChildren = true;

                //可以增加权限
                //o.ResultsAuthorize = request => request.HttpContext.User.IsInRole("Admin");
                //o.UserIdProvider = request => request.HttpContext.User.Identity.Name;
            });
        }
    }
}
