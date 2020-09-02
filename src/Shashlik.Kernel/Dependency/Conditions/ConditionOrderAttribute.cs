using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.Kernel.Dependency.Conditions
{

    /// <summary>
    /// 条件序号,优先级从小到大执行
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ConditionOrderAttribute : Attribute
    {
        /// <summary>
        /// 条件序号,优先级从小到大执行
        /// </summary>
        /// <param name="order">条件序号,优先级从小到大执行</param>
        public ConditionOrderAttribute(int order)
        {
            Order = order;
        }

        public int Order { get; set; }
    }
}
