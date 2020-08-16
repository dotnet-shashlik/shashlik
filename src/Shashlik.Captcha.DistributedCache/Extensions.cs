﻿using Shashlik.Features.VerifyCode;
using Shashlik.Kernel.Dependency;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shashlik.Kernel;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;
using Shashlik.Utils.Common;
using Shashlik.Utils.Extensions;

namespace Shashlik.Features
{
    public static class Extensions
    {
        /// <summary>
        /// 增加通用分布式缓存的功能服务(验证码)
        /// </summary>
        /// <param name="kernelBuilder"></param>
        /// <returns></returns>
        public static IKernelService AddFeatureByDistributedCache(
            this IKernelService kernelBuilder,
            Action<VerifyCodeOptions> action)
        {
            kernelBuilder.Services.Configure(action);
            kernelBuilder.Services.AddSingleton<IVerifyCodeFeature, DistributedCacheVerifyCodeFeature>();
            return kernelBuilder;
        }

        internal static async Task<T> GetObjectAsync<T>(this IDistributedCache cache, string key)
            where T : class
        {
            var content = await cache.GetStringAsync(key);
            if (content.IsNullOrWhiteSpace())
                return null;
            return JsonHelper.Deserialize<T>(content);
        }

        internal static async Task SetObjectAsync(this IDistributedCache cache, string key, object obj, int expireSeconds)
        {
            await cache.SetStringAsync(key, obj.ToJson(), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expireSeconds)
            });
        }
    }
}
