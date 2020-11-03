using Microsoft.Extensions.DependencyInjection.Extensions;
using Shashlik.Kernel;
using Shashlik.Kernel.Attributes;

// ReSharper disable ConvertIfStatementToConditionalTernaryExpression

// ReSharper disable InvertIf

namespace Shashlik.EfCore
{
    /// <summary>
    /// 自动注册嵌套事务/自动注册ef实体类,IServiceAutowire装配顺序500
    /// </summary>
    [Order(500)]
    public class EfCoreAutowire : IServiceAutowire
    {
        public void Configure(IKernelServices kernelService)
        {
            kernelService.Services.TryAddScoped(typeof(IEfNestedTransaction<>), typeof(DefaultEfNestedTransaction<>));
        }
    }
}