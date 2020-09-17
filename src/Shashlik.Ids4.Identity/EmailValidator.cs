using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Shashlik.Captcha;
using Shashlik.Identity;
using Shashlik.Identity.Entities;
using Shashlik.Ids4.Identity.Extend;
using Shashlik.Utils.Extensions;

namespace Shashlik.Ids4.Identity
{
    /// <summary>
    /// 邮件验证码验证器,邮件地址唯一有效
    /// </summary>
    public class EmailValidator : IExtensionGrantValidator
    {
        private readonly ICaptcha _captcha;

        private readonly ShashlikUserManager _userManager;

        private readonly Ids4IdentityOptions _options;

        private readonly List<IUserCreated> _userCreatedList;
        private readonly List<IUserCreating> _userCreatingList;

        public EmailValidator(List<IUserCreating> userCreatingList, List<IUserCreated> userCreatedList, IOptions<Ids4IdentityOptions> options, ShashlikUserManager userManager, ICaptcha captcha)
        {
            _userCreatingList = userCreatingList;
            _userCreatedList = userCreatedList;
            _options = options.Value;
            _userManager = userManager;
            _captcha = captcha;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
           //TODO:...
        }

        public string GrantType => Consts.EMailGrantType;
    }
}