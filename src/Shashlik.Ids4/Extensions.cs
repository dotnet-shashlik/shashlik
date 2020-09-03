using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Sbt.Ids4
{
    public static class Extensions
    {
        //IReferenceTokenStore

        /// <summary>
        /// identity server
        /// </summary>
        /// <param name="services"></param>
        /// <param name="publicOrigin">issuer</param>
        /// <param name="clientsConfiguration"></param>
        /// <param name="apiResourceConfiguration"></param>
        /// <param name="identityResourceConfiguration"></param>
        /// <param name="redisOperationalStoreBuilder"></param>
        /// <param name="redisCacheBuilder"></param>
        public static void AddIds4(
            this IServiceCollection services,
            string publicOrigin,
            IConfigurationSection clientsConfiguration,
            IConfigurationSection apiResourceConfiguration,
            IConfigurationSection identityResourceConfiguration
            )
        {
            //var api = apiResourceConfiguration.Get<IEnumerable<ApiResource>>();
            //var re = apiResourceConfiguration.Get<IEnumerable<IdentityResource>>();

            // 配置ids4服务
            services.AddIdentityServer(r =>
                {
                    r.PublicOrigin = publicOrigin;
                })
                .AddDeveloperSigningCredential()
                .AddExtensionGrantValidator<SmsValidator>()
                .AddExtensionGrantValidator<PwdValidator>()
                .AddExtensionGrantValidator<TwoFactorValidator>()
                //.AddExtensionGrantValidator<WxValidator>()
                .AddInMemoryClients(clientsConfiguration)
                .AddInMemoryApiResources(apiResourceConfiguration)
                .AddInMemoryIdentityResources(identityResourceConfiguration)
                // 使用自定义的存储
                .AddPersistedGrantStore<EfPersistedGrantStore>()
                // 使用自定义的存储
                .AddDeviceFlowStore<EfDeviceFlowStore>()
                // ids4使用自定义的用户档案服务
                .Services.AddScoped<IProfileService, ProfileService>()
                ;
        }
    }
}
