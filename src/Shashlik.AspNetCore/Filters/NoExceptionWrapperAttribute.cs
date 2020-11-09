using System;

namespace Shashlik.AspNetCore.Filters
{
    /// <summary>
    /// 不要进行自动异常处理
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class NoExceptionWrapperAttribute : Attribute
    {
    }
}