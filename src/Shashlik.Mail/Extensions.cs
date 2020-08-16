using System;
using System.Collections.Generic;
using System.Text;
using Shashlik.Kernel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shashlik.Mail
{
    public static class Extensions
    {
        public static IKernelService AddAliyunMail(this IKernelService kernelBuilder, IConfiguration configuration)
        {
            var services = kernelBuilder.Services;
            services.Configure<AliyunDmOptions>(configuration);
            services.AddTransient<IMail, AliyunMail>();
            return kernelBuilder;
        }
    }
}
