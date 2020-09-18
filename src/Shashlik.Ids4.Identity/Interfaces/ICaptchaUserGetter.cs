using System.Collections.Specialized;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Shashlik.Identity;
using Shashlik.Identity.Entities;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Ids4.Identity.Interfaces
{
    /// <summary>
    /// 验证码方式获取用户的方法定义,自动注册为scoped
    /// </summary>
    public interface ICaptchaUserGetter : IScoped
    {
        /// <summary>
        /// 验证码类型,手机/邮件,可自行扩展
        /// </summary>
        string Type { get; }

        /// <summary>
        /// 验证unifier,返回0表示参数正确
        /// </summary>
        /// <param name="unifier"></param>
        /// <returns></returns>
        int Validate(string unifier);

        /// <summary>
        /// 如何查找用户
        /// </summary>
        /// <param name="unifier"></param>
        /// <param name="manager"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        Task<Users> FindByUnifierAsync(string unifier, ShashlikUserManager manager, NameValueCollection postData);
    }
}