using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;
using Shashlik.Sms.Exceptions;
using Shashlik.Sms.Options;
using Shashlik.Utils.Extensions;
using AliyunClient = AlibabaCloud.SDK.Dysmsapi20170525.Client;
using AliyunConfig = AlibabaCloud.OpenApiClient.Models.Config;
using AliyunSendSmsRequest = AlibabaCloud.SDK.Dysmsapi20170525.Models.SendSmsRequest;

namespace Shashlik.Sms.Aliyun
{
    /// <summary>
    /// 阿里云短信
    /// </summary>
    [Singleton]
    [ConditionOnProperty(typeof(bool), "Shashlik.Sms." + nameof(SmsOptions.Enable), true, DefaultValue = true)]
    public class AliyunSmsSender : AbstractSmsProvider
    {
        private IOptions<AliyunSmsOptions> AliyunOptions { get; }
        private AliyunClient Client { get; }

        public AliyunSmsSender(IOptions<AliyunSmsOptions> aliyunOptions, ISmsLimit smsLimit, IOptionsMonitor<SmsOptions> sOptions)
            : base(smsLimit, sOptions)
        {
            AliyunOptions = aliyunOptions;
            Client = new AliyunClient(new AliyunConfig
            {
                AccessKeyId = AliyunOptions.Value.AppId,
                AccessKeySecret = AliyunOptions.Value.AppKey,
                Endpoint = AliyunOptions.Value.EndPoint
            });
        }

        public override string ProviderName => "aliyun";

        public override void SendCheck(string phone, string subject, params string[] args)
        {
            base.SendCheck(phone, subject, args);
            var template = Options.CurrentValue.Templates.GetOrDefault(subject);
            if (template!.Params is not null && template.Params.Length != args.Length)
                throw new ArgumentException($"sms template of \"{subject}\" [Params] except argument count: {template.Params?.Length ?? 0}.",
                    nameof(args));
        }

        public override async Task<string> SendAsync(IEnumerable<string> phones, string subject, params string[] args)
        {
            var phoneArr = phones as string[] ?? phones.ToArray();
            if (phoneArr.IsNullOrEmpty())
                throw new ArgumentException("phones argument can not be empty.", nameof(phones));
            if (string.IsNullOrWhiteSpace(subject)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(subject));

            var template = Options.CurrentValue.Templates.GetOrDefault(subject);
            if (template == null)
                throw new SmsTemplateException(phoneArr, subject);
            if (string.IsNullOrWhiteSpace(template.TemplateId))
                throw new SmsTemplateException(phoneArr, subject);
            if (string.IsNullOrWhiteSpace(template.Sign))
                throw new SmsTemplateException(phoneArr, subject);

            var @params = new Dictionary<string, string>();
            if (template.Params is not null && template.Params.Length != args.Length)
                throw new ArgumentException($"sms template of \"{subject}\" [Params] except argument count: {template.Params?.Length ?? 0}.",
                    nameof(args));

            if (template.Params is not null)
                for (int i = 0; i < args.Length; i++)
                    @params[template.Params[i]] = args[i];


            var res = await Client.SendSmsAsync(new AliyunSendSmsRequest
            {
                PhoneNumbers = phoneArr.Join(","),
                SignName = template.Sign,
                TemplateCode = template.TemplateId,
                TemplateParam = @params.ToJson()
            });

            if (res.Body.Code.EqualsIgnoreCase("OK"))
                // 发送成功, 返回可用于后续查询的BizId
                return res.Body.BizId;

            throw new SmsServerException(phoneArr, $"aliyun sms send failed: {res.Body.Message}", res.Body.Code, res);
        }
    }
}