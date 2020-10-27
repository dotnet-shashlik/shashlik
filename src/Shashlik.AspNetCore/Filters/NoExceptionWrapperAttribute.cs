using System;

namespace Shashlik.AspNetCore.Filters
{
    /// <summary>
    /// 不要进行自动异常处理
    /// </summary>
    public class NoExceptionWrapperAttribute : Attribute
    {
    }
}