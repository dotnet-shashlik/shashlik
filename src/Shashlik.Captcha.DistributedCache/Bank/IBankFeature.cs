using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Guc.Features.Bank
{
    /// <summary>
    /// 银行基础数据功能
    /// </summary>
    public interface IBankFeature : Guc.Utils.Dependency.ITransient
    {
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="bank"></param>
        /// <returns></returns>
        Task Insert(Banks bank);

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="banks"></param>
        /// <returns></returns>
        Task Insert(IEnumerable<Banks> banks);

        /// <summary>
        /// 获取所有银行数据
        /// </summary>
        /// <returns></returns>
        Task<List<Banks>> Get();

        /// <summary>
        /// 根据银行id获取银行数据
        /// </summary>
        /// <param name="bankId"></param>
        /// <returns></returns>
        Task<Banks> Get(string bankId);
    }
}
