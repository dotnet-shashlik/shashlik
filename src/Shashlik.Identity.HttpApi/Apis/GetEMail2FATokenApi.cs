﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.Captcha;
using Shashlik.Identity;
using Shashlik.Ids4.Identity.Extend;
using Shashlik.Kernel;
using Shashlik.Utils.Extensions;

namespace Shashlik.Ids4.Identity.HttpApi
{
    /// <summary>
    /// 邮件双因子验证验证码发送默认实现
    /// </summary>
    public class GetEMail2FATokenApi : IAutowiredConfigureAspNetCore
    {
        private readonly Ids4IdentityOptions _identityOptions;

        public GetEMail2FATokenApi(IOptions<Ids4IdentityOptions> identityOptions)
        {
            _identityOptions = identityOptions.Value;
        }

        public void Configure(IApplicationBuilder app)
        {
            app.Map(_identityOptions.GetEMail2FATokenApi, builder =>
            {
                builder.Run(async context =>
                {
                    var email = context.Request.Form["email"].ToString();
                    if (!email.IsMatch(Utils.Consts.Regexs.Email))
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        return;
                    }
                    var captcha = context.RequestServices.GetService<ICaptcha>();
                    var code = await captcha.Build(Consts._2FAPurpose, email, _identityOptions.CodeLength);
                    var emailSender = context.RequestServices.GetService<IIdentityEMailSender>();
                    await emailSender.Send(Consts._2FAPurpose, email, code.Code);
                    //TODO:auto response by accept type
                });
            });
        }
    }
}
