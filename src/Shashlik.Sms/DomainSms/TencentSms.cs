using Shashlik.Utils.Extensions;
using Microsoft.Extensions.Logging;
using qcloudsms_csharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shashlik.Sms.DomainSms
{
    /// <summary>
    /// 手机短信
    /// </summary>
    class TencentSms : ISmsInvoker
    {
        public TencentSms(ILogger<TencentSms> logger)
        {
            Logger = logger;
        }

        public int SmsDomain => 2;
        ILogger<TencentSms> Logger { get; }

        public void Send(SmsDomainConfig options, IEnumerable<string> phones, string subject, params string[] args)
        {
            var template = options.Templates.FirstOrDefault(r => r.SmsSubject.EqualsIgnoreCase(subject));
            if (template == null)
                throw new Exception($"短信发送失败,未定义的短信类型:{subject}");
            if (template.TemplateId.IsNullOrWhiteSpace())
            {
                Logger.LogWarning($"未配置短信模版,无法发送[{subject}]短信");
                return;
            }
            try
            {
                var res = new SmsMultiSender(options.AppId.ConvertTo<int>(), options.AppKey)
                    .sendWithParam("86", phones.ToArray(), template.TemplateId.ConvertTo<int>(), args, template.SmsSign, "", "");
                if (res.result != 0)
                    throw new SmsDomainException($"腾讯短信发送失败,{res.errMsg}:{phones.Join(",")}");
            }
            catch (Exception ex)
            {
                throw new SmsDomainException(ex.Message, ex);
            }
        }
    }
}
