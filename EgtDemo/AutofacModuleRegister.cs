using Autofac;
using Autofac.Extras.DynamicProxy;
using EgtDemo.Common.AOP;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;

namespace EgtDemo
{
    internal class AutofacModuleRegister : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var basePath = AppContext.BaseDirectory;
            var serviceDLLFile = Path.Combine(basePath, "EgtDemo.Service.dll");
            var repositoryDLLFile = Path.Combine(basePath, "EgtDemo.Repository.dll");

            if (!(File.Exists(serviceDLLFile) && File.Exists(repositoryDLLFile)))
            {
                var msg = "项目DLL文件不存在，请先编译后运行！";
                throw new FileNotFoundException(msg);
            }

            builder.RegisterType<BlogLogAOP>();
            builder.RegisterType<BlogCacheAOP>();

            //注册服务
            var serviceAssembly = Assembly.LoadFrom(serviceDLLFile);
            var repositoryAssembly = Assembly.LoadFrom(repositoryDLLFile);

            /*builder.RegisterAssemblyTypes(new Assembly[] { serviceAssembly, repositoryAssembly })
                .AsImplementedInterfaces()
                .InstancePerDependency()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(BlogLogAOP));*/
            builder.RegisterAssemblyTypes(repositoryAssembly)
                .AsImplementedInterfaces()
                .InstancePerDependency();

            builder.RegisterAssemblyTypes(serviceAssembly)
                .AsImplementedInterfaces()
                .InstancePerDependency()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(BlogLogAOP), typeof(BlogCacheAOP));
        }
    }
}