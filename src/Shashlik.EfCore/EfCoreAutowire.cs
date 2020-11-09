using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
        public EfCoreAutowire(IOptions<EfCoreOptions> options)
        {
            Options = options;
        }

        private IOptions<EfCoreOptions> Options { get; }

        public void Configure(IKernelServices kernelService)
        {
            if (Options.Value.Enable)
                return;
            kernelService.Services.AddScoped(typeof(IEfNestedTransaction<>), typeof(DefaultEfNestedTransaction<>));
        }
    }
}