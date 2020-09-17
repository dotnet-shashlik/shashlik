using System.Collections.Specialized;
using Shashlik.Identity.Entities;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Ids4.Identity.Extend
{
    /// <summary>
    /// 用户创建中,userId还没有,<see cref="Ids4IdentityOptions.CreateUserIfNotExistsOnPhoneValidator"/>或<see cref="Ids4IdentityOptions.CreateUserIfNotExistsOnEmailValidator"/>为true时必须有实现类,来定义创建时用户字段的赋值逻辑,自动注册为scoped
    /// </summary>
    public interface IUserCreating : IScoped
    {
        /// <summary>
        /// 方法执行
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="clientId">客户端id</param>
        /// <param name="postData">前端提交的数据</param>
        void Action(Users user, string clientId, NameValueCollection postData);
    }
}