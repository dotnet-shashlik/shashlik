using System.Collections.Specialized;
using System.Threading.Tasks;
using Shashlik.Identity;
using Shashlik.Identity.Entities;

namespace Shashlik.Ids4.Identity
{
    /// <summary>
    /// 验证码方式获取用户的方法定义,自动注册为scoped
    /// </summary>
    public interface IIdentityUserFinder
    {
        /// <summary>
        /// 如何查找用户
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="manager"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        Task<Users?> FindByUnifierAsync(string identity, ShashlikUserManager manager, NameValueCollection postData);
    }
}