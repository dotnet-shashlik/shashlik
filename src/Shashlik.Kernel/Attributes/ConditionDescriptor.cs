namespace Shashlik.Kernel.Attributes
{
    public class ConditionDescriptor
    {
        public ConditionDescriptor(ConditionBaseAttribute condition, int order)
        {
            Condition = condition;
            Order = order;
        }

        /// <summary>
        /// 条件数据
        /// </summary>
        public ConditionBaseAttribute Condition { get; }

        /// <summary>
        /// 条件顺序
        /// </summary>
        public int Order { get; }
    }
}