using System.Collections.Specialized;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Shashlik.Identity;
using Shashlik.Identity.Entities;
using Shashlik.Identity.Options;
using Shashlik.Kernel.Dependency;
using Shashlik.Kernel.Dependency.Conditions;

namespace Shashlik.Ids4.Identity
{
    /// <summary>
    /// 默认的用户查找类,一次根据手机号码/邮件地址/身份证号码/用户名查找用户
    /// </summary>
    [ConditionDependsOnMissing(typeof(IIdentityUserFinder))]
    public class DefaultIIdentityUserFinder : IIdentityUserFinder, IScoped
    {
        public DefaultIIdentityUserFinder(IOptions<IdentityOptions> identityOptions,
            IOptions<IdentityOptionsExtends> identityOptionsExtends)
        {
            IdentityOptions = identityOptions;
            IdentityOptionsExtends = identityOptionsExtends;
        }

        private IOptions<IdentityOptions> IdentityOptions { get; }
        private IOptions<IdentityOptionsExtends> IdentityOptionsExtends { get; }

        public async Task<Users?> FindByUnifierAsync(string identity, ShashlikUserManager manager,
            NameValueCollection postData)
        {
            Users user = null;
            if (IdentityOptionsExtends.Value.PhoneNumberUnique)
                user = await manager.FindByPhoneNumberAsync(identity);
            if (user == null && IdentityOptions.Value.User.RequireUniqueEmail)
                user = await manager.FindByEmailAsync(identity);
            if (user == null)
                user = await manager.FindByNameAsync(identity);
            if (user == null && IdentityOptionsExtends.Value.IdCardUnique)
                user = await manager.FindByIdCardAsync(identity);

            return user;
        }
    }
}