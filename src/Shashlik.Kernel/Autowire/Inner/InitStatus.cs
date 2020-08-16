using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.Kernel.Autowire
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
    }
}
