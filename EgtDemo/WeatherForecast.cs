using EgtDemo.Model;
using System;

namespace EgtDemo
{
    /// <summary>
    /// 天气预报
    /// </summary>
    public class WeatherForecast
    {
        /// <summary>
        /// 预报时间
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 摄氏温度
        /// </summary>
        public int TemperatureC { get; set; }

        /// <summary>
        /// 华氏温度
        /// </summary>
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        /// <summary>
        /// 总计
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 自定义实例
        /// </summary>
        public Demo Demo { get; set; }
    }

    public enum ApiVersion
    {
        /// <summary>
        /// 版本一
        /// </summary>
        v1 = 1,
        /// <summary>
        /// 版本二
        /// </summary>
        v2 = 2
    }
}
