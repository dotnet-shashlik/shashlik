using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.Kernel;

namespace Shashlik.Captcha
{
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
