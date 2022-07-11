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

            services.AddCorsSetup();  //����

            /*
             * ��ȡConfiguration�����ļ�������
             */
            //1��ֱ����Configuration��ȥ����
            //BaseDBConfig.ConnectionString = Configuration.GetSection("AppSettings:SqlServerConnection").Value;
            BaseDBConfig.ConnectionString = Configuration.GetConnectionString("DefaultConnStr");

            //2����װ����
            Appsetting1 configuration1 = new Appsetting1();
            Configuration.Bind(configuration1);
            services.AddSingleton(configuration1);

            //3�����ˣ����ֽڵ㣩
            services.Configure<Logging>(Configuration.GetSection(nameof(Logging)));

            //4������ʵ�����ö���Դ�Ͷ���Ŀ����ConfigurationBuilder���μ�EgtDemo.Common.Helper ��Appsettings�� 

            //5����Pragram.cs�ļ������ã��ӹ�core�Դ���IConfiguration������Iloggerһ��

            //��ȡ�������� ��ע��
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IMemoryCache, MemoryCache>();
            services.AddScoped<ICaching, MemoryCaching>();

            //����jwt��������������net core�ڲ������Զ���ת����ϵͳ�Դ���������ʽ
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthorization(o =>
            {
                //���ڽ�ɫRoles����Ȩ����(��)
                o.AddPolicy("AdminOrUser", c => { c.RequireRole("Admin", "User").Build(); });
                o.AddPolicy("AdminAndUser", c => { c.RequireRole("Admin").RequireRole("User").Build(); });

                //��������Claims����Ȩ����(һ��)
                o.AddPolicy("Email", c => { c.RequireClaim(JwtRegisteredClaimNames.Email).Build(); });

                //��������Requirements����Ȩ����(�Զ��塪������)
                o.AddPolicy("BaseOnRequirement", c => { c.Requirements.Add(new PermissionRequirement { Name = "JimmyWu" }); });
            });
            //Ҫ��������Զ��帴�Ӳ�����Ч������ע������ķ��񣡣���
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
                //    Title = $"{ApiName} �ӿ��ĵ�����Net Core 3.1",
                //    Description = $"{ApiName} HTTP API V1",
                //    Contact = new OpenApiContact { Name = ApiName, Email = "Jimmy2090@qq.com", Url = new Uri("https://www.jianshu.com/u/94102b59cc2a"), },
                //    License = new OpenApiLicense { Name = ApiName, Url = new Uri("https://www.jianshu.com/u/94102b59cc2a") }
                //});
                typeof(ApiVersion).GetEnumNames().ToList().ForEach(v =>
                {
                    o.SwaggerDoc(v, new OpenApiInfo
                    {
                        Version = v,
                        Title = $"{ApiName} �ӿ��ĵ�����Net Core 3.1",
                        Description = $"{ApiName} HTTP API " + v,
                        Contact = new OpenApiContact { Name = ApiName, Email = "Jimmy2090@qq.com", Url = new Uri("https://www.jianshu.com/u/94102b59cc2a"), },
                        License = new OpenApiLicense { Name = ApiName, Url = new Uri("https://www.jianshu.com/u/94102b59cc2a") }
                    });
                });
                //�Խӿڵ�չʾ�������򣺴˴��Ǹ��ݽӿڵ����·��������
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
                    Description = "JWT��Ȩ��֤",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
            });

            //ע��MiniProfiler���ܼ�����
            services.AddMiniProfilerSetup();

            //ע��SignalR
            services.AddSignalR();//.AddNewtonsoftJsonProtocol();
        }

        //asp.net core 3.x ֮������ע���������׼д��
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

                //��swaggerҳ�����MiniProfiler���ܼ��
                //o.HeadContent = "<h3><a href='https://www.baidu.com'>��ת���ٶ�</a></h3>";
                o.HeadContent = @$"<script async='async' id='mini-profiler' src='/profiler/includes.min.js?v=4.2.22+gcc91adf599'
                                data-version='4.2.22+gcc91adf599' data-path='/profiler/'
                                data-current-id='4ec7c742-49d4-4eaf-8281-3c1e0efa748a' data-ids='4ec7c742-49d4-4eaf-8281-3c1e0efa748' 
                                data-position='Left' data-authorized='true' data-max-traces='15' data-toggle-shortcut='Alt+P'
                                data-trivial-milliseconds='2.0' data-ignored-duplicate-execute-types='Open,OpenAsync,Close,CloseAsync'>
                                </script>";

                // ���Ҫ��swagger��ҳ�����ó������Զ����ҳ�棬�ǵ�����ַ�����д����������.index.html
                //o.IndexStream = () => GetType().GetTypeInfo().Assembly.GetManifestResourceStream("Blog.Core.index.html");
            });

            /*
             * ע�⣺�����ʹ���� app.UserMvc() ���� app.UseHttpsRedirection()������м����һ��Ҫ�� app.UseCors() д
             * �����ǵ��ϱߣ��Ƚ��п����ٽ��� Http ���󣬷������ʾ����ʧ�ܡ�
             * ��Ϊ�����������漰�� Http����ģ�����㲻�����ֱ��ת������mvc���ǿ϶�����
            */
            app.UseRouting();

            app.UseCors("LimitRequests"); //����
            //�����configServices���õ��� services.AddCors()�������������ã���ô�������м�������ã����£�
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
