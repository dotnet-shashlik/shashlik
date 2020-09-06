using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Shashlik.Kernel;
using Shashlik.Kernel.Autowired;
using Shashlik.Sms.Domains;
using Shashlik.Sms.Options;

namespace Shashlik.Sms
{
    /// <summary>
    /// 短信服务自动配置
    /// </summary>
    public class SmsConfigure : IAutowiredConfigureServices
    {
        public SmsConfigure(IOptions<SmsOptions> options)
        {
            Options = options;
        }

        private IOptions<SmsOptions> Options { get; }

        public void ConfigureServices(IKernelServices kernelService)
        {
            if (!Options.Value.Enable)
                return;

            if (Options.Value.UseEmptySms) kernelService.Services.TryAddSingleton<ISms, EmptySms>();
            else kernelService.Services.TryAddSingleton<ISms, DefaultSms>();

            if (Options.Value.EnableDistributedCacheLimit)
                kernelService.Services.TryAddSingleton<ISmsLimit, DistributedCacheSmsLimit>();
            else
            {
                kernelService.Services.AddMemoryCache();
                kernelService.Services.TryAddSingleton<ISmsLimit, MemorySmsLimit>();
            }

            kernelService.Services.AddTransient<ISmsDomain, AliSms>();
            kernelService.Services.AddTransient<ISmsDomain, TencentSms>();
        }
    }
}