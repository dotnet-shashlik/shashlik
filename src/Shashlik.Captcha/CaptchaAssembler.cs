using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.Kernel;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Captcha
{
    [Transient]
    internal class CaptchaAssembler : IServiceAssembler
    {
        public IOptions<CaptchaOptions> Options { get; }

        public CaptchaAssembler(IOptions<CaptchaOptions> options)
        {
            Options = options;
        }

        public void Configure(IKernelServices kernelServices)
        {
            if (!Options.Value.Enable)
                return;

            kernelServices.Services.AddMemoryCache();
        }
    }
}