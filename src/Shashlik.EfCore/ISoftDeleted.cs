using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shashlik.EfCore
{
    /// <summary>
    /// 软删除标记
    /// </summary>
    public interface ISoftDeleted<T> where T : struct
    {
        /// <summary>
        ///  是否已删除
        /// </summary>
        bool IsDeleted { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        T? DeleteTime { get; set; }
    }
}
