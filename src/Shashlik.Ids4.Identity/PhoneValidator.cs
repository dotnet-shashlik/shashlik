using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Shashlik.Captcha;
using Shashlik.Identity;
using Shashlik.Identity.Entities;
using Shashlik.Ids4.Identity.Extend;
using Shashlik.Utils.Extensions;

namespace Shashlik.Ids4.Identity
{
    /// <summary>
    /// 手机短信验证码验证码
    /// </summary>
    public class PhoneValidator : IExtensionGrantValidator
    {
        private readonly ICaptcha _captcha;

        private readonly ShashlikUserManager _userManager;

        private readonly Ids4IdentityOptions _options;

        private readonly List<IUserCreated> _userCreatedList;
        private readonly List<IUserCreating> _userCreatingList;

        public PhoneValidator(ICaptcha captcha, ShashlikUserManager userManager, IOptions<Ids4IdentityOptions> options,
            List<IUserCreated> userCreatedList, List<IUserCreating> userCreatingList)
        {
            _captcha = captcha;
            _userManager = userManager;
            _userCreatedList = userCreatedList;
            _userCreatingList = userCreatingList;
            _options = options.Value;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var phone = context.Request.Raw.Get("phone");
            var code = context.Request.Raw.Get("code");
            var clientId = context.Request.Raw.Get("client_id");
            if (phone.IsNullOrWhiteSpace())
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = "手机号码不能为空"
                };
                return;
            }

            if (!phone.IsMatch(Utils.Consts.Regexs.MobilePhoneNumber))
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = "手机号码格式错误"
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

            if (!await _captcha.IsValid(Consts.LoginPurpose, phone, code))
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = "验证码错误"
                };
                return;
            }

            var user = await _userManager.FindByPhoneNumber(phone);
            if (user == null)
            {
                // create user if enabled
                if (_options.CreateUserIfNotExistsOnPhoneValidator)
                {
                    user = new Users()
                    {
                        UserName = phone,
                        PhoneNumber = phone,
                        PhoneNumberConfirmed = true,
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
                        Error = "手机号码不存在"
                    };
                    return;
                }
            }

            var sub = await _userManager.GetUserIdAsync(user);
            context.Result = new GrantValidationResult(sub, GrantType);
        }

        public string GrantType => Consts.PhoneGrantType;
    }
}