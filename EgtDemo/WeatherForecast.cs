using EgtDemo.Model;
using System;

namespace EgtDemo
{
    /// <summary>
    /// ����Ԥ��
    /// </summary>
    public class WeatherForecast
    {
        /// <summary>
        /// Ԥ��ʱ��
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// �����¶�
        /// </summary>
        public int TemperatureC { get; set; }

        /// <summary>
        /// �����¶�
        /// </summary>
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        /// <summary>
        /// �ܼ�
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// �Զ���ʵ��
        /// </summary>
        public Demo Demo { get; set; }
    }

    public enum ApiVersion
    {
        /// <summary>
        /// �汾һ
        /// </summary>
        v1 = 1,
        /// <summary>
        /// �汾��
        /// </summary>
        v2 = 2
    }
}
