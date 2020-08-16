using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.Bank
{
    /// <summary>
    /// 银行卡类型
    /// </summary>
    public enum CardType
    {
        /// <summary>
        /// 储蓄卡
        /// </summary>
        DC = 1,
        /// <summary>
        /// 信用卡
        /// </summary>
        CC = 2,
        /// <summary>
        /// 准贷记卡
        /// </summary>
        SCC = 3,
        /// <summary>
        /// 预付费卡
        /// </summary>
        PC = 4,
        /// <summary>
        /// 未知
        /// </summary>
        Unknown = -1
    }
}
