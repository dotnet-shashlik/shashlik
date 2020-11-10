﻿#nullable enable
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
using Shashlik.Kernel.Dependency;
using Shashlik.Utils.Extensions;

namespace Shashlik.Ids4.IdentityInt32
{
    /// <summary>
    /// 默认的用户查找类,依次根据手机号码/邮件地址/身份证号码/用户名查找用户
    /// </summary>
    [ConditionDependsOnMissing(typeof(IIdentityUserFinder))]
    public class DefaultIIdentityUserFinder : IIdentityUserFinder
    {
        public DefaultIIdentityUserFinder(IOptions<IdentityOptions> identityOptions,
            IOptions<IdentityUserExtendsOptions> identityOptionsExtends, ILogger<DefaultIIdentityUserFinder> logger)
        {
            IdentityOptions = identityOptions;
            IdentityOptionsExtends = identityOptionsExtends;
            Logger = logger;
        }

        private IOptions<IdentityOptions> IdentityOptions { get; }
        private IOptions<IdentityUserExtendsOptions> IdentityOptionsExtends { get; }
        private ILogger<DefaultIIdentityUserFinder> Logger { get; }

        public async Task<Users?> FindByIdentityAsync(
            string identity,
            IEnumerable<string> allowSignInSources,
            ShashlikUserManager<Users, int> manager,
            NameValueCollection postData)
        {
            if (identity == null) throw new ArgumentNullException(nameof(identity));
            if (allowSignInSources == null) throw new ArgumentNullException(nameof(allowSignInSources));
            if (manager == null) throw new ArgumentNullException(nameof(manager));
            if (postData == null) throw new ArgumentNullException(nameof(postData));

            Users? user = null;
            var signInSources = allowSignInSources.ToList();
            if (signInSources.IsNullOrEmpty())
            {
                Logger.LogWarning(
                    "SignInSource is empty, check your configuration: Shashlik.Ids4.Identity.PasswordSignInSources/Shashlik.Ids4.Identity.CaptchaSignInSources.");
                return null;
            }

            if (user == null && signInSources.Contains(ShashlikIds4IdentityConsts.UsernameSource))
                user = await manager.FindByNameAsync(identity);

            // 手机号唯一才能使用手机号码登录
            if (IdentityOptionsExtends.Value.RequireUniquePhoneNumber &&
                signInSources.Contains(ShashlikIds4IdentityConsts.PhoneSource))
                user = await manager.FindByPhoneNumberAsync(identity);

            // 邮件地址唯一才能使用邮件登录
            if (user == null
                && IdentityOptions.Value.User.RequireUniqueEmail
                && signInSources.Contains(ShashlikIds4IdentityConsts.EMailSource))
                user = await manager.FindByEmailAsync(identity);

            if (user == null
                && IdentityOptionsExtends.Value.RequireUniqueIdCard
                && signInSources.Contains(ShashlikIds4IdentityConsts.IdCardSource))
                user = await manager.FindByIdCardAsync(identity);

            return user;
        }
    }
}