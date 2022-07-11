using EgtDemo.Common.Helper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EgtDemo.Extensions
{
    public static class CorsSetup
    {
        /// <summary>
        /// Cors的方式解决跨域
        /// </summary>
        /// <param name="services"></param>
        public static void AddCorsSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            var orgins = Appsettings.app(new string[] { "Startup", "Cors", "IPs" }).Split(',');
            services.AddCors(o =>
            {
                o.AddPolicy("LimitRequests", policy =>
                 {
                     policy.WithOrigins(orgins)
                         .AllowAnyHeader()
                         .AllowAnyMethod();
                 });
            });
        }
    }
}
