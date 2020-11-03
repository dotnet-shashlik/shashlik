using System;
// ReSharper disable RedundantAttributeUsageProperty

namespace Shashlik.Kernel.Attributes
{
    /// <summary>
    /// 条件序号,优先级从小到大执行, 装配类型有效
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class OrderAttribute : Attribute
    {
        /// <summary>
        /// 条件序号,优先级从小到大执行
        /// </summary>
        /// <param name="order">条件序号,优先级从小到大执行</param>
        public OrderAttribute(int order)
        {
            Order = order;
        }

        public int Order { get; }
    }
}