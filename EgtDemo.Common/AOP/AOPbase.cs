﻿using Castle.DynamicProxy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EgtDemo.Common.AOP
{
    public abstract class AOPbase : IInterceptor
    {
        public abstract void Intercept(IInvocation invocation);

        /// <summary>
        /// 自定义缓存的key
        /// </summary>
        /// <param name="invocation"></param>
        /// <returns></returns>
        protected string CustomCacheKey(IInvocation invocation)
        {
            var typeName = invocation.TargetType.Name; //获取被调用方法的所在类的类型
            var methodName = invocation.Method.Name; //被调用方法的名称
            var args = invocation.Arguments.Select(GetArgumentValue).Take(3).ToList(); //获取参数列表

            string key = $"{typeName}:{methodName}:";
            foreach(var param in args)
            {
                key = $"{key}{param}:";
            }

            return key.TrimEnd(':');
        }

        /// <summary>
        /// object转string类型
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected static string GetArgumentValue(object arg)
        {
            if (arg is DateTime || arg is DateTime?)
                return ((DateTime)arg).ToString("yyyyMMddHHmmss");

            if (arg is string || arg is ValueType || arg is Nullable)
                return arg.ToString();

            if (arg != null)
            {
                if (arg.GetType().IsClass)
                {
                    //return MD5Encrypt16(JsonConvert.SerializeObject(arg));
                    return JsonConvert.SerializeObject(arg);
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 16位MD5加密
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        /*public static string MD5Encrypt16(string password)
        {
            var md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(password)), 4, 8);
            t2 = t2.Replace("-", string.Empty);
            return t2;
        }*/
    }
}
