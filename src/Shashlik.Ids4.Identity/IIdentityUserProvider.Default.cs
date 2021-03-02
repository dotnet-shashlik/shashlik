using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shashlik.Identity;
using Shashlik.Identity.Options;
using Shashlik.Kernel.Attributes;
using Shashlik.Utils.Extensions;

namespace Shashlik.Ids4.Identity
{
    /// <summary>
    /// 默认的用户查找类,依次根据手机号码/邮件地址/身份证号码/用户名查找用户
    /// </summary>
    [ConditionDependsOnMissing(typeof(IIdentityUserProvider))]
    [ConditionOnProperty(typeof(bool), "Shashlik.Ids4.Identity.Enable", true, DefaultValue = true)]
    public class DefaultIIdentityUserProvider : IIdentityUserProvider
    {
        public DefaultIIdentityUserProvider(IOptions<IdentityOptions> identityOptions,
            IOptions<IdentityUserExtendsOptions> identityOptionsExtends, ILogger<DefaultIIdentityUserProvider> logger)
        {
            IdentityOptions = identityOptions;
            IdentityOptionsExtends = identityOptionsExtends;
            Logger = logger;
        }

        private IOptions<IdentityOptions> IdentityOptions { get; }
        private IOptions<IdentityUserExtendsOptions> IdentityOptionsExtends { get; }
        private ILogger<DefaultIIdentityUserProvider> Logger { get; }

        public async Task<IIdentityUser?> FindByIdentityAsync(
            string identity,
            IEnumerable<string> allowSignInSources,
            IShashlikUserManager manager,
            NameValueCollection postData)
        {
            if (identity is null) throw new ArgumentNullException(nameof(identity));
            if (allowSignInSources is null) throw new ArgumentNullException(nameof(allowSignInSources));
            if (manager is null) throw new ArgumentNullException(nameof(manager));
            if (postData is null) throw new ArgumentNullException(nameof(postData));

            IIdentityUser? user = null;
            var signInSources = allowSignInSources.ToList();
            if (signInSources.IsNullOrEmpty())
            {
                Logger.LogWarning(
                    "SignInSource is empty, check your configuration: Shashlik.Ids4.Identity.PasswordSignInSources/Shashlik.Ids4.Identity.CaptchaSignInSources");
                return null;
            }

            if (user is null && signInSources.Contains(ShashlikIds4IdentityConsts.UsernameSource))
                user = await manager.FindIdentityUserByNameAsync(identity);

            // 手机号唯一才能使用手机号码登录
            if (IdentityOptionsExtends.Value.RequireUniquePhoneNumber &&
                signInSources.Contains(ShashlikIds4IdentityConsts.PhoneSource))
                user = await manager.FindIdentityUserByPhoneNumberAsync(identity);

            // 邮件地址唯一才能使用邮件登录
            if (user is null
                && IdentityOptions.Value.User.RequireUniqueEmail
                && signInSources.Contains(ShashlikIds4IdentityConsts.EMailSource))
                user = await manager.FindIdentityUserByEmailAsync(identity);

            if (user is null
                && IdentityOptionsExtends.Value.RequireUniqueIdCard
                && signInSources.Contains(ShashlikIds4IdentityConsts.IdCardSource))
                user = await manager.FindIdentityUserByIdCardAsync(identity);

            return user;
        }
    }
}