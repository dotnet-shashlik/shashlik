using Shashlik.AspNetCore.PatchUpdate;
using Shashlik.Kernel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Shashlik.AspNetCore
{
    public static class Extensions
    {
        /// <summary>
        /// 增加Shashlik.AspNetCore,PatchUpdateBinder
        /// </summary>
        /// <param name="ShashlikUtilsBuilder"></param>
        public static IKernelBuilder AddAspNetCore(this IKernelBuilder kernelBuilder)
        {
            kernelBuilder.Services.AddControllers(options =>
            {
                options.ModelBinderProviders.Insert(0, new PatchUpdateBinderProvider());
            })
           .AddNewtonsoftJson();
            return kernelBuilder;
        }
    }
}
