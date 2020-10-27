using Microsoft.Extensions.DependencyInjection.Extensions;
using Shashlik.Kernel;

// ReSharper disable ConvertIfStatementToConditionalTernaryExpression

// ReSharper disable InvertIf

namespace Shashlik.EfCore
{
    /// <summary>
    /// 自动注册嵌套事务/自动注册ef实体类
    /// </summary>
    public class EfCoreAutowire : IServiceAutowire
    {
        public void Configure(IKernelServices kernelService)
        {
            kernelService.AddEfEntityMappings();
            kernelService.Services.TryAddScoped(typeof(IEfNestedTransaction<>), typeof(DefaultEfNestedTransaction<>));
        }
    }
}