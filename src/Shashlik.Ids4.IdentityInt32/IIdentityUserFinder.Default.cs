#nullable enable
using System.Collections.Specialized;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Shashlik.Identity;
using Shashlik.Identity.Options;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Ids4.IdentityInt32
{
    /// <summary>
    /// 默认的用户查找类,依次根据手机号码/邮件地址/身份证号码/用户名查找用户
    /// </summary>
    [ConditionDependsOnMissing(typeof(IIdentityUserFinder))]
    public class DefaultIIdentityUserFinder : IIdentityUserFinder, IScoped
    {
        public DefaultIIdentityUserFinder(IOptions<IdentityOptions> identityOptions,
            IOptions<IdentityUserExtendsOptions> identityOptionsExtends,
            IOptions<ShashlikIds4IdentityOptions> shashlikIds4IdentityOptions)
        {
            IdentityOptions = identityOptions;
            IdentityOptionsExtends = identityOptionsExtends;
            ShashlikIds4IdentityOptions = shashlikIds4IdentityOptions;
        }

        private IOptions<IdentityOptions> IdentityOptions { get; }
        private IOptions<IdentityUserExtendsOptions> IdentityOptionsExtends { get; }
        private IOptions<ShashlikIds4IdentityOptions> ShashlikIds4IdentityOptions { get; }

        public async Task<Users?> FindByIdentityAsync(string identity, ShashlikUserManager<Users, int> manager,
            NameValueCollection postData)
        {
            Users? user = null;
            // 手机号唯一才能使用手机号码登录
            if (IdentityOptionsExtends.Value.RequireUniquePhoneNumber &&
                ShashlikIds4IdentityOptions.Value.CaptchaSignInSources.Contains(ShashlikIds4IdentityConsts.PhoneSource))
                user = await manager.FindByPhoneNumberAsync(identity);
            // 手机号唯一才能使用手机号码登录
            if (IdentityOptions.Value.User.RequireUniqueEmail &&
                ShashlikIds4IdentityOptions.Value.CaptchaSignInSources.Contains(ShashlikIds4IdentityConsts.EMailSource))
                user = await manager.FindByEmailAsync(identity);
            if (user == null &&
                ShashlikIds4IdentityOptions.Value.CaptchaSignInSources.Contains(ShashlikIds4IdentityConsts
                    .UsernameSource))
                user = await manager.FindByNameAsync(identity);
            if (user == null && IdentityOptionsExtends.Value.RequireUniqueIdCard &&
                ShashlikIds4IdentityOptions.Value.CaptchaSignInSources.Contains(ShashlikIds4IdentityConsts.IdCardSource)
            )
                user = await manager.FindByIdCardAsync(identity);

            return user;
        }
    }
}