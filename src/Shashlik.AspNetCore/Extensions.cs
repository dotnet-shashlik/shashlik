using Guc.AspNetCore.PatchUpdate;
using Guc.Kernel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Guc.AspNetCore
{
    public static class Extensions
    {
        /// <summary>
        /// 增加Guc.AspNetCore,PatchUpdateBinder
        /// </summary>
        /// <param name="gucUtilsBuilder"></param>
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
