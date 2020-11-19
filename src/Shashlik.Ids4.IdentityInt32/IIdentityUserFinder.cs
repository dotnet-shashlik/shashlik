using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Shashlik.Identity;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Ids4.IdentityInt32
{
    /// <summary>
    /// 通过identity获取用户的方法定义,自动注册为scoped
    /// </summary>
    [Scoped]
    public interface IIdentityUserFinder
    {
        /// <summary>
        /// 如何查找用户
        /// </summary>
        /// <param name="identity">登录凭证,username/phoneNumber/email数据等</param>
        /// <param name="allowSignInSources">允许的登录源,username/phone/email/idcard</param>
        /// <param name="manager">userManager</param>
        /// <param name="postData">前端提交的原始数据</param>
        /// <returns></returns>
        Task<Users?> FindByIdentityAsync(
            string identity,
            IEnumerable<string> allowSignInSources,
            ShashlikUserManager<Users, int> manager,
            NameValueCollection postData);
    }
}