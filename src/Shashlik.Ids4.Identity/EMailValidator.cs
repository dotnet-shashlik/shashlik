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
    /// 邮件验证码验证器
    /// </summary>
    public class EMailValidator : IExtensionGrantValidator
    {
        private readonly ICaptcha _captcha;

        private readonly ShashlikUserManager _userManager;

        private readonly Ids4IdentityOptions _options;

        private readonly List<IUserCreated> _userCreatedList;
        private readonly List<IUserCreating> _userCreatingList;

        public EMailValidator(List<IUserCreating> userCreatingList, List<IUserCreated> userCreatedList, IOptions<Ids4IdentityOptions> options, ShashlikUserManager userManager, ICaptcha captcha)
        {
            _userCreatingList = userCreatingList;
            _userCreatedList = userCreatedList;
            _options = options.Value;
            _userManager = userManager;
            _captcha = captcha;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var email = context.Request.Raw.Get("email");
            var code = context.Request.Raw.Get("code");
            var clientId = context.Request.Raw.Get("client_id");
            if (email.IsNullOrWhiteSpace())
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = "Email不能为空"
                };
                return;
            }

            if (!email.IsMatch(Utils.Consts.Regexs.Email))
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = "Email格式错误"
                };
                return;
            }

            if (code.IsNullOrWhiteSpace())
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = "验证码不能为空"
                };
                return;
            }

            if (!await _captcha.IsValid(Consts.LoginPurpose, email, code))
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = "验证码错误"
                };
                return;
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // create user if enabled
                if (_options.CreateUserIfNotExistsOnPhoneValidator)
                {
                    user = new Users()
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true,
                        Gender = Gender.Unknown
                    };
                    if (_userCreatingList.Count > 0)
                    {
                        foreach (var userCreating in _userCreatingList)
                        {
                            userCreating.Action(user, clientId, context.Request.Raw);
                        }
                    }

                    await _userManager.CreateAsync(user);
                    if (_userCreatedList.Count > 0)
                    {
                        foreach (var userCreated in _userCreatedList)
                        {
                            userCreated.Action(user, clientId, context.Request.Raw);
                        }
                    }
                }
                else
                {
                    context.Result = new GrantValidationResult
                    {
                        IsError = true,
                        Error = "Email不存在"
                    };
                    return;
                }
            }

            var sub = await _userManager.GetUserIdAsync(user);
            context.Result = new GrantValidationResult(sub, GrantType);
        }

        public string GrantType => Consts.EMailGrantType;
    }
}