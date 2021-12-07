using System.Threading.Tasks;
using System.Transactions;
using AspectCore.DynamicProxy;

namespace Shashlik.EfCore.Transactional
{
    /// <summary>
    /// TransactionScope分布式事务(XA)特性
    /// </summary>
    public class TransactionScopeAttribute : AbstractInterceptorAttribute
    {
        /// <summary>
        /// 默认启用异步流
        /// </summary>
        public TransactionScopeAsyncFlowOption AsyncFlowOption { get; set; } = TransactionScopeAsyncFlowOption.Enabled;

        /// <summary>
        /// 事务传播选项
        /// </summary>
        public TransactionScopeOption ScopeOption { get; set; } = TransactionScopeOption.Required;

        /// <summary>
        /// 事务选项
        /// </summary>
        public TransactionOptions? TransactionOptions { get; set; }


        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            using TransactionScope tran = !TransactionOptions.HasValue
                ? new TransactionScope(ScopeOption, AsyncFlowOption)
                : new TransactionScope(ScopeOption, TransactionOptions.Value, AsyncFlowOption);

            await next(context);
            tran.Complete();
        }
    }
}