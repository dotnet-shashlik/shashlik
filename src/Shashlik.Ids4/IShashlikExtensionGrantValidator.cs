using IdentityServer4.Validation;

namespace Shashlik.Ids4
{
    /// <summary>
    /// 自定义的grant type,将自动加载不用手动配置
    /// </summary>
    public interface IShashlikExtensionGrantValidator : IExtensionGrantValidator
    {
    }
}