using System;
using System.Collections.Generic;
using System.Text;

namespace Guc.AspNetCore
{
    /// <summary>
    /// 发生异常时的处理类
    /// </summary>
    public interface IOnException : Guc.Kernel.Dependency.ITransient
    {
        void OnException(Exception exception);
    }
}
