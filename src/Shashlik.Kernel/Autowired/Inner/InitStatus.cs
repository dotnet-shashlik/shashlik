namespace Shashlik.Kernel.Autowired.Inner
{
    /// <summary>
    /// 初始化状态
    /// </summary>
    enum InitStatus
    {
        /// <summary>
        /// 等待执行
        /// </summary>
        Waiting = 0,

        /// <summary>
        /// 挂起中,递归运行性,用于循环依赖检测
        /// </summary>
        Hangup = 1,

        /// <summary>
        /// 已执行完成
        /// </summary>
        Done = 2,

        /// <summary>
        /// 标识移除,暂时无用
        /// </summary>
        Removed = -1
    }
}