#nullable enable
using System.Collections.Specialized;
using System.Threading.Tasks;
using Shashlik.Identity;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Ids4.IdentityInt32
{
    /// <summary>
    /// 通过identity获取用户的方法定义,自动注册为scoped
    /// </summary>
    public interface IIdentityUserFinder : IScoped
    {
        /// <summary>
        /// 如何查找用户
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="manager"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        Task<Users?> FindByIdentityAsync(string identity, ShashlikUserManager<Users, int> manager,
            NameValueCollection postData);
    }
}