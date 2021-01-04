using System;

namespace Shashlik.Kernel.Attributes
{
    /// <summary>
    /// 标识此类作为主要实现类，即GetService()获取到此类型。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PrimaryAttribute : Attribute
    {
    }
}