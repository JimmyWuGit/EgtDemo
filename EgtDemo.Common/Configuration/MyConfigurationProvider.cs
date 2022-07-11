using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace EgtDemo.Common.Configuration
{
    public class MyConfigurationProvider: ConfigurationProvider
    {
        /// <summary>
        /// 从远程配置中心读取配置信息（微服务、分布式开发）
        /// </summary>
        public async override void Load()
        {
            var response = "";
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:5001");
                response = await client.GetStringAsync("/api/Dtos/GetConfigs");
            }
            catch (Exception)
            {
                throw;
            }

            if (string.IsNullOrWhiteSpace(response))
                throw new Exception("数据未响应");

            var configs = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(response);
            base.Data = new ConcurrentDictionary<string, string>();
            configs.ForEach(a =>
            {
                Data.Add(a);
            });
        }
    }
}
