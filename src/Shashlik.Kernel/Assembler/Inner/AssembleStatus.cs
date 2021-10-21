namespace Shashlik.Kernel.Assembler.Inner
{
    /// <summary>
    /// 初始化状态
    /// </summary>
    internal enum AssembleStatus
    {
        /// <summary>
        /// 等待执行
        /// </summary>
        Waiting = 0,

        /// <summary>
        /// 挂起中,递归运行时,用于循环依赖检测
        /// </summary>
        Hangup = 1,

        /// <summary>
        /// 已执行完成
        /// </summary>
        Done = 2
    }
}