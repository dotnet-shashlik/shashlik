using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Guc.Features.VerifyCode;
using Guc.Utils.Extensions;
using Microsoft.Extensions.Options;
using Guc.TemplateString;
using Guc.EventBus;
using Microsoft.Extensions.Logging;

namespace Sbt.Ids4
{
    /// <summary>
    /// 微信登录
    /// </summary>
    public class WxValidator : IExtensionGrantValidator
    {
        public string GrantType => "wx";

        public Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            throw new NotImplementedException();
        }
    }
}