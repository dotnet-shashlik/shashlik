﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using qcloudsms_csharp;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;
using Shashlik.Sms.Exceptions;
using Shashlik.Sms.Options;
using Shashlik.Utils.Extensions;

namespace Shashlik.Sms.Domains
{
    /// <summary>
    /// 手机短信
    /// </summary>
    [ConditionOnProperty(typeof(bool), "Shashlik.Sms.Enable", true, DefaultValue = true)]
    public class TencentSms : ISmsDomain
    {
        public TencentSms(ILogger<TencentSms> logger)
        {
            Logger = logger;
        }

        public string SmsDomain => "tencent";
        private ILogger<TencentSms> Logger { get; }

        public void Send(SmsDomainConfig options, IEnumerable<string> phones, string subject,
            params string[] args)
        {
            var template = options.Templates.FirstOrDefault(r => r.Subject.EqualsIgnoreCase(subject));
            if (template is null)
                throw new Exception($"短信发送失败,未定义的短信类型:{subject}");
            if (template.TemplateId.IsNullOrWhiteSpace())
            {
                Logger.LogWarning($"未配置短信模版,无法发送[{subject}]短信");
                return;
            }

            try
            {
                var list = phones.ToList();
                var res = new SmsMultiSender(options.AppId.ParseTo<int>(), options.AppKey)
                    .sendWithParam("86", list.ToArray(), template.TemplateId.ParseTo<int>(), args, template.Sign, "",
                        "");
                if (res.result != 0)
                    throw new SmsDomainException($"腾讯短信发送失败,{res.errMsg}:{list.Join(",")}");
            }
            catch (Exception ex)
            {
                throw new SmsDomainException(ex.Message, ex);
            }
        }
    }
}