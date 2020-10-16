﻿using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel;
using Shashlik.Kernel.Autowired;

namespace Shashlik.JsonPatch.AspNetCore
{
    public class JsonPatchConfigure : IAutowiredConfigureServices
    {
        public void ConfigureServices(IKernelServices kernelService)
        {
            kernelService.Services.AddControllers(options =>
                {
                    // 增加模型绑定器
                    options.ModelBinderProviders.Add(new PatchUpdateBinderProvider());
                })
                // 必须使用NewtonsoftJson
                .AddNewtonsoftJson();
        }
    }
}