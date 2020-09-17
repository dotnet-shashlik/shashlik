using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shashlik.Kernel;
using Shashlik.Utils.Extensions;
using Shashlik.Kernel.Autowired;
using Shashlik.Utils.Helpers;

// ReSharper disable ConvertIfStatementToConditionalTernaryExpression

// ReSharper disable InvertIf

namespace Shashlik.EfCore
{
    /// <summary>
    /// 自动注册嵌套事务/自动注册ef实体类
    /// </summary>
    public class EfCoreConfigure : IAutowiredConfigureServices
    {
        public void ConfigureServices(IKernelServices kernelService)
        {
            kernelService.AddEfEntityMappings();
            kernelService.Services.TryAddScoped<IEfNestedTransactionWrapper, DefaultEfNestedTransactionWrapper>();

            var dbContextTypes =
                AssemblyHelper.GetFinalSubTypes(typeof(DbContext), kernelService.ScanFromDependencyContext);
            if (dbContextTypes.IsNullOrEmpty())
                return;

            // 自动注册所有的DbContext嵌套事务
            foreach (var item in dbContextTypes)
            {
                kernelService.Services.TryAddScoped(
                    typeof(IEfNestedTransaction<>).MakeGenericType(item),
                    typeof(DefaultEfNestedTransaction<>).MakeGenericType(item)
                );
            }
        }
    }
}