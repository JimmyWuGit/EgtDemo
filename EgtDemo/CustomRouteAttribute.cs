using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System;

namespace EgtDemo
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method,AllowMultiple =true,Inherited =true)]
    public class CustomRouteAttribute:RouteAttribute,IApiDescriptionGroupNameProvider
    {
        /// <summary>
        /// 分组名，实现IApiDescriptionGroupNameProvider接口
        /// </summary>
        public string GroupName { get; set; }

        public CustomRouteAttribute(string actionName="[action]")
            :base("/api/{version}/[controller]/"+actionName)
        {

        }

        public CustomRouteAttribute(ApiVersion version,string actionName="[action]")
            :base($"/api/{version}/[controller]/"+actionName)
        {
            GroupName = version.ToString();
        }
    }
}
