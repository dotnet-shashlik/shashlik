using System;
using System.Collections.Generic;
using System.Text;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Guc.Kernel.Dependency;
using Guc.Mail.Aliyun;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Guc.Mail
{
    public class AliyunMail : IMail
    {
        private AliyunDmOptions Options { get; }
        private IAcsClient Client { get; }

        private ILogger<AliyunMail> Logger { get; }

        public AliyunMail(IOptions<AliyunDmOptions> options, ILogger<AliyunMail> logger)
        {
            Options = options.Value;
            Logger = logger;
            IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", Options.AccessId, Options.AccessKey);
            Client = new DefaultAcsClient(profile);

        }

        public void Send(string address, string subject, string content)
        {
            var request = new SingleSendMailRequest
            {
                AccountName = Options.AccountName,
                FromAlias = Options.FromAlias,
                AddressType = 1,
                ReplyToAddress = true,
                ToAddress = address,
                Subject = subject,
                HtmlBody = content,
            };
            try
            {
                var response = Client.GetAcsResponse(request);
            }
            catch (ClientException e)
            {
                Logger.LogError(e, "阿里云邮件发送失败");
            }
        }
    }
}
