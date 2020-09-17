﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.Captcha;
using Shashlik.Ids4.Identity.Extend;
using Shashlik.Kernel;
using Shashlik.Utils.Extensions;

namespace Shashlik.Ids4.Identity.HttpApi
{
    /// <summary>
    /// 手机短信登录验证码发送默认实现
    /// </summary>
    public class GetPhoneLoginCodeApi : IAutowiredConfigureAspNetCore
    {
        private readonly Ids4IdentityOptions _identityOptions;

        public GetPhoneLoginCodeApi(IOptions<Ids4IdentityOptions> identityOptions)
        {
            _identityOptions = identityOptions.Value;
        }

        public void Configure(IApplicationBuilder app)
        {
            app.Map(_identityOptions.GetPhoneLoginCodeApi, builder =>
            {
                builder.Run(async context =>
                {
                    var phone = context.Request.Form["phone"].ToString();
                    if (!phone.IsMatch(Utils.Consts.Regexs.MobilePhoneNumber))
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        return;
                    }
                    var captcha = context.RequestServices.GetService<ICaptcha>();
                    var code = await captcha.Build(Consts.LoginPurpose, phone, _identityOptions.CodeLength);
                    var smsSender = context.RequestServices.GetService<IIdentitySmsSender>();
                    await smsSender.Send(Consts.LoginPurpose, phone, code.Code);
                    //TODO:auto response by accept type
                });
            });
        }
    }
}
