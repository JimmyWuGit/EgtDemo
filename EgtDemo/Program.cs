using Autofac.Extensions.DependencyInjection;
using EgtDemo.Common.Configuration;
using EgtDemo.Common.Helper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.IO;

namespace EgtDemo
{
    public class Program
    {
        static Logger log = default(Logger);
        public static void Main(string[] args)
        {
            string conn = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=conn;Integrated Security=True;
                            Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;
                            MultiSubnetFailover=False";
            string tabName = "Logs";


            using (log = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(Path.Combine("Logs", @"Serilog20220330.log"), LogEventLevel.Debug,
                                rollingInterval: RollingInterval.Day)
                .WriteTo.MSSqlServer(conn, sinkOptions: new MSSqlServerSinkOptions
                {
                    AutoCreateSqlTable = true,
                    TableName = tabName
                }, restrictedToMinimumLevel: LogEventLevel.Information)
                .CreateLogger())
            {
                //log.Information("Serilog��¼��info�������־������");
                //ģ��ķ�ʽ�����־
                //var model = new { Name = "Jimmy", Age = 17 };
                //log.Information("Data:{0}", model);
                //log.Information("Data:{@0}", model);
            }

            //Log.Logger = new LoggerConfiguration()
            //    .MinimumLevel.Warning()
            //    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            //    .WriteTo.Console()
            //    .CreateLogger();
            //Log.Warning("Serilog��¼��warning�������־������");
            //Log.CloseAndFlush(); //����using�Ļ������ֶ��رպ����

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory()) //�������ע��Ĺ�����
                //.ConfigureAppConfiguration(builder =>
                //{
                //    builder.Add(new MyConfigurationSource()); //���������ģ������Ǳ�ķ���������ȡ����
                //})
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).UseSerilog(log);
    }
}
