using System;

namespace Shashlik.AspNetCore.Filters
{
    /// <summary>
    /// 不要自动包装响应结果
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class NoResponseWrapperAttribute : Attribute
    {
    }
}