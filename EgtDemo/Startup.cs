using Autofac;
using EgtDemo.Common;
using EgtDemo.Common.Configuration;
using EgtDemo.Common.Helper;
using EgtDemo.Extensions;
using EgtDemo.JWTHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Text;

namespace EgtDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public string ApiName { get; set; } = "EgtDemo";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddCorsSetup();  //跨域

            /*
             * 获取Configuration配置文件的内容
             */
            //1、直接用Configuration类去操作
            //BaseDBConfig.ConnectionString = Configuration.GetSection("AppSettings:SqlServerConnection").Value;
            BaseDBConfig.ConnectionString = Configuration.GetConnectionString("DefaultConnStr");

            //2、封装到类
            Appsetting1 configuration1 = new Appsetting1();
            Configuration.Bind(configuration1);
            services.AddSingleton(configuration1);

            //3、过滤（部分节点）
            services.Configure<Logging>(Configuration.GetSection(nameof(Logging)));

            //4、（能实现配置多来源和多项目共享）ConfigurationBuilder：参见EgtDemo.Common.Helper 的Appsettings类 

            //5、在Pragram.cs文件中配置，接管core自带的IConfiguration，就像Ilogger一样

            //获取上下文类 的注册
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IMemoryCache, MemoryCache>();
            services.AddScoped<ICaching, MemoryCaching>();

            //采用jwt的声明，而不让net core内部进行自动的转化成系统自带的声明方式
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthorization(o =>
            {
                //基于角色Roles的授权策略(简单)
                o.AddPolicy("AdminOrUser", c => { c.RequireRole("Admin", "User").Build(); });
                o.AddPolicy("AdminAndUser", c => { c.RequireRole("Admin").RequireRole("User").Build(); });

                //基于声明Claims的授权策略(一般)
                o.AddPolicy("Email", c => { c.RequireClaim(JwtRegisteredClaimNames.Email).Build(); });

                //基于需求Requirements的授权策略(自定义——复杂)
                o.AddPolicy("BaseOnRequirement", c => { c.Requirements.Add(new PermissionRequirement { Name = "JimmyWu" }); });
            });
            //要想上面的自定义复杂策略生效，必须注册下面的服务！！！
            services.AddSingleton<IAuthorizationHandler, RequirementHandler>();

            var secret = "jimmy2022jimmy2022";
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
            services.AddAuthentication("Bearer").AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,

                    ValidateIssuer = true,
                    ValidIssuer = "http://localhost:5000",

                    ValidateAudience = true,
                    ValidAudience = "http://localhost:5002",

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                    RequireExpirationTime = true
                };

            });

            var bathPath = ApplicationEnvironment.ApplicationBasePath;
            //var bathPath2 = AppContext.BaseDirectory;
            services.AddSwaggerGen(o =>
            {
                //o.SwaggerDoc("v1", new OpenApiInfo
                //{
                //    Version = "v1",
                //    Title = $"{ApiName} 接口文档——Net Core 3.1",
                //    Description = $"{ApiName} HTTP API V1",
                //    Contact = new OpenApiContact { Name = ApiName, Email = "Jimmy2090@qq.com", Url = new Uri("https://www.jianshu.com/u/94102b59cc2a"), },
                //    License = new OpenApiLicense { Name = ApiName, Url = new Uri("https://www.jianshu.com/u/94102b59cc2a") }
                //});
                typeof(ApiVersion).GetEnumNames().ToList().ForEach(v =>
                {
                    o.SwaggerDoc(v, new OpenApiInfo
                    {
                        Version = v,
                        Title = $"{ApiName} 接口文档——Net Core 3.1",
                        Description = $"{ApiName} HTTP API " + v,
                        Contact = new OpenApiContact { Name = ApiName, Email = "Jimmy2090@qq.com", Url = new Uri("https://www.jianshu.com/u/94102b59cc2a"), },
                        License = new OpenApiLicense { Name = ApiName, Url = new Uri("https://www.jianshu.com/u/94102b59cc2a") }
                    });
                });
                //对接口的展示进行排序：此处是根据接口的相对路径来排序
                o.OrderActionsBy(a => a.RelativePath);

                var xmlPath = Path.Combine(bathPath, "EgtDemo.xml");
                o.IncludeXmlComments(xmlPath, true);
                var xmlModelPath = Path.Combine(bathPath, "EgtDemo.Model.xml");
                o.IncludeXmlComments(xmlModelPath);

                o.OperationFilter<AddResponseHeadersFilter>();
                o.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                o.OperationFilter<SecurityRequirementsOperationFilter>();

                o.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "JWT授权验证",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
            });

            //注册MiniProfiler性能监测服务
            services.AddMiniProfilerSetup();

            //注册SignalR
            services.AddSignalR();//.AddNewtonsoftJsonProtocol();
        }

        //asp.net core 3.x 之后，依赖注入的容器标准写法
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new AutofacModuleRegister());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(o =>
            {
                typeof(ApiVersion).GetEnumNames().OrderByDescending(e => e).ToList().ForEach(v =>
                {
                    o.SwaggerEndpoint($"/swagger/{v}/swagger.json", $"{ApiName} {v}");
                    o.RoutePrefix = "";
                });
                //o.SwaggerEndpoint($"/swagger/v1/swagger.json", $"{ApiName} v1");
                //o.RoutePrefix = "";

                //在swagger页面加入MiniProfiler性能监测
                //o.HeadContent = "<h3><a href='https://www.baidu.com'>跳转至百度</a></h3>";
                o.HeadContent = @$"<script async='async' id='mini-profiler' src='/profiler/includes.min.js?v=4.2.22+gcc91adf599'
                                data-version='4.2.22+gcc91adf599' data-path='/profiler/'
                                data-current-id='4ec7c742-49d4-4eaf-8281-3c1e0efa748a' data-ids='4ec7c742-49d4-4eaf-8281-3c1e0efa748' 
                                data-position='Left' data-authorized='true' data-max-traces='15' data-toggle-shortcut='Alt+P'
                                data-trivial-milliseconds='2.0' data-ignored-duplicate-execute-types='Open,OpenAsync,Close,CloseAsync'>
                                </script>";

                // 如果要将swagger首页，设置成我们自定义的页面，记得这个字符串的写法：程序集名.index.html
                //o.IndexStream = () => GetType().GetTypeInfo().Assembly.GetManifestResourceStream("Blog.Core.index.html");
            });

            /*
             * 注意：如果你使用了 app.UserMvc() 或者 app.UseHttpsRedirection()这类的中间件，一定要把 app.UseCors() 写
             * 在它们的上边，先进行跨域，再进行 Http 请求，否则会提示跨域失败。
             * 因为这两个都是涉及到 Http请求的，如果你不跨域就直接转发或者mvc，那肯定报错。
            */
            app.UseRouting();

            app.UseCors("LimitRequests"); //跨域
            //如果在configServices中用的是 services.AddCors()，而不进行配置，那么可以在中间件中配置，如下：
            //app.UseCors(policy =>
            //{
            //    policy.WithOrigins(new string[] { "http://127.0.0.1:5001", "http://localhost:5001" });
            //    policy.AllowAnyHeader();
            //    policy.AllowAnyMethod();
            //});

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiniProfiler();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chatHub");
            });
        }
    }
}
