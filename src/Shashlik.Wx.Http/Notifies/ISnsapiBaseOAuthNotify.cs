﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Guc.Wx.Notifies
{
    /// <summary>
    /// wx授权登录 回调处理
    /// </summary>
    public interface ISnsapiBaseOAuthNotify : Guc.Kernel.Dependency.ITransient
    {
        /// <summary>
        /// 优先级 从小大
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// 类型,不区分大小写
        /// </summary>
        string Type { get; }

        /// <summary>
        /// 处理逻辑
        /// </summary>
        /// <param name="openid">用户微信openid</param>
        /// <param name="type">用途类型</param>
        /// <param name="attach">附加数据</param>
        /// <param name="extendUrlArgs">额外附加到前端跳转地址上的参数,没有返回null,默认会附加openid/type/attach</param>
        Task<IEnumerable<KeyValuePair<string, object>>> Handle(string openid, string type, string attach);
    }
}
