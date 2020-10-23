using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shashlik.Kernel.Attributes;

namespace Shashlik.Kernel.Test.TestClasses
{
    /// <summary>
    /// 测试自定义条件注册
    /// </summary>
    public class TestConditionAttribute : Attribute, IConditionBase
    {
        public TestConditionAttribute(bool shouldDependency)
        {
            ShouldDependency = shouldDependency;
        }

        public bool ShouldDependency { get; set; }

        public bool ConditionOn(IServiceCollection services, IConfiguration rootConfiguration,
            IHostEnvironment hostEnvironment)
        {
            return ShouldDependency;
        }
    }
}