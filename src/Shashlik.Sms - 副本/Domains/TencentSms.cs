using System;
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
        public string SmsDomain => "tencent";

        public void Send(SmsDomainConfig options, IEnumerable<string> phones, string subject,
            params string[] args)
        {
            var template = options.Templates.FirstOrDefault(r => r.Subject.EqualsIgnoreCase(subject));
            try
            {
                var list = phones.ToList();
                var res = new SmsMultiSender(options.AppId.ParseTo<int>(), options.AppKey)
                    .sendWithParam(options.Region.IsNullOrWhiteSpace() ? "86" : options.Region, list.ToArray(), template!.TemplateId.ParseTo<int>(),
                        args, template.Sign, "",
                        "");
                if (res.result != 0)
                    throw new SmsDomainException($"tencent cloud sms send failed, error:{res.errMsg}, phone: {list.Join(",")}.");
            }
            catch (Exception ex)
            {
                throw new SmsDomainException(ex.Message, ex);
            }
        }
    }
}