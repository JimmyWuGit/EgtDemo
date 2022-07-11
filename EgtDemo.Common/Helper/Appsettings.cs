using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration.Memory;
using System;
using System.Collections.Concurrent;

namespace EgtDemo.Common.Helper
{
    public class Appsettings
    {
        static IConfiguration Configuration { get; set; }
        static Appsettings()
        {
            var dictionary = new ConcurrentDictionary<string, string>()
            {
                ["Chinese"] = "赵春影",
                ["English"] = "DreamyZhao",
                ["Love"] = "Darling"
            };

            Configuration = new ConfigurationBuilder()
                //既可以从缓存中读取可以从自定义文件中读取配置信息
                .Add(new MemoryConfigurationSource { InitialData = dictionary })
                .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
                //.SetBasePath("")  //可实现多项目共享一个目录下的配置文件
                .Build()
                //.GetSection("Logging") //也可以进行过滤，只需部分节点
                ;
        }

        public static string app(params string[] sections)
        {
            try
            {
                var val = string.Empty;
                for(int i = 0; i < sections.Length; i++)
                {
                    val += sections[i] + ":";
                }
                return Configuration[val.TrimEnd(':')];
            }
            catch (Exception)
            {

                return "";
            }
        }

        public static string GetVal(string key)
        {
            return Configuration[key];
        }
    }
}
