using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Guc.Version
{
    public interface IVersion : Guc.Kernel.Dependency.ITransient
    {
        /// <summary>
        /// 优先级
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// 版本id
        /// </summary>
        string VersionId { get; }

        /// <summary>
        /// 版本备注
        /// </summary>
        string Desc { get; }

        /// <summary>
        /// 执行更新操作
        /// </summary>
        Task Update();
    }
}
