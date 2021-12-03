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
    public class AliyunSmsSender : ISmsSender
    {
        private IOptions<AliyunSmsOptions> AliyunOptions { get; }
        private IOptionsMonitor<SmsOptions> SOptions { get; }
        private ISmsLimit SmsLimit { get; }
        private AliyunClient Client { get; }
        private HashSet<string> LimitErrorCode { get; }

        public AliyunSmsSender(IOptions<AliyunSmsOptions> aliyunOptions, ISmsLimit smsLimit, IOptionsMonitor<SmsOptions> sOptions)
        {
            AliyunOptions = aliyunOptions;
            SmsLimit = smsLimit;
            SOptions = sOptions;
            LimitErrorCode = new HashSet<string>
            {
                "isv.DAY_LIMIT_CONTROL",
                "isv.BUSINESS_LIMIT_CONTROL"
            };

            Client = new AliyunClient(new AliyunConfig
            {
                AccessKeyId = AliyunOptions.Value.AppId,
                AccessKeySecret = AliyunOptions.Value.AppKey,
                Endpoint = AliyunOptions.Value.EndPoint
            });
        }

        public bool SendCaptchaLimitCheck(string phone)
        {
            return SmsLimit.CanSend(phone);
        }

        public async Task<string> SendCaptchaAsync(string phone, string subject, params string[] args)
        {
            if (!SendCaptchaLimitCheck(phone))
                throw new SmsLimitException();
            try
            {
                var requestId = await SendAsync(phone, subject, args);
                SmsLimit.SendDone(phone);
                return requestId;
            }
            catch (SmsServerException e) when (e.ErrorCode is not null && LimitErrorCode.Contains(e.ErrorCode))
            {
                throw new SmsLimitException(e.Message, e);
            }
        }

        public async Task<string> SendAsync(string phone, string subject, params string[] args)
        {
            if (string.IsNullOrWhiteSpace(phone)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(phone));
            if (string.IsNullOrWhiteSpace(subject)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(subject));
            return await SendAsync(new[] { phone }, subject, args);
        }

        public async Task<string> SendAsync(IEnumerable<string> phones, string subject, params string[] args)
        {
            var phoneArr = phones as string[] ?? phones.ToArray();
            if (phoneArr.IsNullOrEmpty())
                throw new ArgumentException("phones argument can not be empty.", nameof(phones));
            if (string.IsNullOrWhiteSpace(subject)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(subject));

            var template = SOptions.CurrentValue.Templates.FirstOrDefault(r => r.Subject == subject);
            if (template == null)
                throw new ArgumentException($"sms template of \"{subject}\" not found.", subject);
            if (string.IsNullOrWhiteSpace(template.TemplateId))
                throw new SmsTemplateException(subject, $"sms template \"{subject}\" [TemplateId] can not be empty.");
            if (string.IsNullOrWhiteSpace(template.Sign))
                throw new SmsTemplateException(subject, $"sms template \"{subject}\" [Sign] can not be empty.");

            var @params = new Dictionary<string, string>();
            if (template.Params is not null && template.Params.Length != args.Length)
                throw new ArgumentException($"sms template of \"{subject}\" [Params] require argument count: {template.Params?.Length ?? 0}.",
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
            throw new SmsServerException($"aliyun sms send failed: {res.Body.Message}", res.Body.Code, res);
        }
    }
}