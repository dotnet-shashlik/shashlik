using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using Shashlik.Captcha;
using Shashlik.Identity;
using Shashlik.Identity.Entities;
using Shashlik.Ids4.Identity.Extend;
using Shashlik.Utils.Extensions;

namespace Shashlik.Ids4.Identity
{
    /// <summary>
    /// 手机短信验证码验证码,手机号码唯一有效
    /// </summary>
    public class PhoneValidator : IExtensionGrantValidator
    {
        private readonly ICaptcha _captcha;
        private readonly ShashlikUserManager _userManager;
        private readonly Ids4IdentityOptions _options;
        private readonly IEnumerable<IUserCreated> _userCreatedList;
        private readonly IEnumerable<IUserCreating> _userCreatingList;

        public PhoneValidator(ICaptcha captcha, ShashlikUserManager userManager, IOptions<Ids4IdentityOptions> options,
            IEnumerable<IUserCreated> userCreatedList, IEnumerable<IUserCreating> userCreatingList)
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
                //TODO:...
            }

            if (!phone.IsMatch(Utils.Consts.Regexs.MobilePhoneNumber))
            {
                //TODO:...
            }

            if (code.IsNullOrWhiteSpace())
            {
                //TODO:...
            }

            if (!await _captcha.IsValid(Consts.LoginPurpose, phone, code))
            {
                //TODO:...
            }

            var user = await _userManager.FindByPhoneNumberAsync(phone);
            if (user == null)
            {
                // create user if enabled
                if (_options.CreateUserIfNotExistsOnPhoneValidator)
                {
                    user = new Users()
                    {
                        PhoneNumber = phone,
                        PhoneNumberConfirmed = true,
                        Gender = Gender.Unknown
                    };
                    if (_userCreatingList.Any())
                    {
                        foreach (var userCreating in _userCreatingList)
                        {
                            userCreating.Action(user, clientId, context.Request.Raw);
                        }
                    }

                    await _userManager.CreateAsync(user);
                    if (_userCreatedList.Any())
                    {
                        foreach (var userCreated in _userCreatedList)
                        {
                            userCreated.Action(user, clientId, context.Request.Raw);
                        }
                    }
                }
                else
                {
                    //TODO:....
                }
            }

            var sub = await _userManager.GetUserIdAsync(user);
            context.Result = new GrantValidationResult(sub, GrantType);
        }

        public string GrantType => Consts.PhoneGrantType;
    }
}