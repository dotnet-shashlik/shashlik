using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.Kernel;
using Shashlik.Kernel.Attributes;
using Shashlik.Sms.Options;

namespace Shashlik.Sms
{
    /// <summary>
    /// 短信服务自动配置,装配顺序300
    /// </summary>
    [Order(300)]
    public class SmsAutowire : IServiceAutowire
    {
        public SmsAutowire(IOptions<SmsOptions> options)
        {
            Options = options;
        }

        private IOptions<SmsOptions> Options { get; }

        public void Configure(IKernelServices kernelService)
        {
            if (!Options.Value.Enable)
                return;

            kernelService.Services.AddMemoryCache();
        }
    }
}