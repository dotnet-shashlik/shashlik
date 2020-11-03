using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Shashlik.Kernel;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Autowired;
using Shashlik.Sms.Domains;
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

            if (Options.Value.UseEmptySms)
                kernelService.Services.TryAddSingleton<ISms, EmptySms>();
            else kernelService.Services.TryAddSingleton<ISms, DefaultSms>();

            if (Options.Value.EnableDistributedCacheLimit)
                kernelService.Services.TryAddSingleton<ISmsLimit, DistributedCacheSmsLimit>();
            else
            {
                kernelService.Services.AddMemoryCache();
                kernelService.Services.TryAddSingleton<ISmsLimit, MemorySmsLimit>();
            }

            kernelService.Services.AddSingleton<ISmsDomain, AliSms>();
            kernelService.Services.AddSingleton<ISmsDomain, TencentSms>();
        }
    }
}