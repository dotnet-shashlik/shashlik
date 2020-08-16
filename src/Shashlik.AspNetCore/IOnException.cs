using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.AspNetCore
{
    /// <summary>
    /// 发生异常时的处理类
    /// </summary>
    public interface IOnException : Shashlik.Kernel.Dependency.ITransient
    {
        void OnException(Exception exception);
    }
}
