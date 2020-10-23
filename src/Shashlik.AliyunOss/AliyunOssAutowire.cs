using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.Kernel;
using Shashlik.Kernel.Attributes;
using Shashlik.Utils.Extensions;

namespace Shashlik.AliyunOss
{
    public class AliyunOssAutowire : IServiceAutowire
    {
        public AliyunOssAutowire(IOptions<AliyunOssOptions> options)
        {
            Options = options.Value;
        }

        private AliyunOssOptions Options { get; }

        public void Configure(IKernelServices kernelService)
        {
            if (!Options.Enable)
                return;
            if (Options.Buckets.HasRepeat(r => r.Bucket))
                throw new InvalidOperationException($"重复的bucket配置");
            if (Options.Buckets.Count(r => r.IsDefault) > 1)
                throw new InvalidOperationException($"默认bucket只能配置一个");
            foreach (var item in Options.Buckets.OrderBy(r => r.IsDefault))
                kernelService.Services.AddSingleton<IAliyunOssProvider>(new AliyunOssDefaultProvider(item));
        }
    }
}