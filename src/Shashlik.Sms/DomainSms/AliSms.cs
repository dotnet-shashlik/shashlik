﻿using System;
using System.Collections.Generic;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
using System.Linq;
using Newtonsoft.Json;
using Shashlik.Utils.Extensions;
using Microsoft.Extensions.Logging;

namespace Shashlik.Sms.DomainSms
{
    /// <summary>
    /// 模板参数规则请使用 ${p1}/${p2},配置模板时按参数顺序来定义.
    /// </summary>
    class AliSms : ISmsInvoker
    {
        public AliSms(ILogger<TencentSms> logger)
        {
            Logger = logger;
        }

        public int SmsDomain => 1;
        ILogger<TencentSms> Logger { get; }

        public void Send(SmsDomainConfig options, IEnumerable<string> phones, string subject, params string[] args)
        {
            var template = options.Templates.FirstOrDefault(r => r.SmsSubject.EqualsIgnoreCase(subject));
            if (template == null)
                throw new SmsArgException($"短信发送失败,未定义的短信类型:{subject}");
            if (template.TemplateId.IsNullOrWhiteSpace())
            {
                Logger.LogWarning($"未配置短信模版,无法发送[{subject}]短信");
                return;
            }

            String product = "Dysmsapi";//短信API产品名称（短信产品名固定，无需修改）
            String domain = "dysmsapi.aliyuncs.com";//短信API产品域名（接口地址固定，无需修改）
            String accessKeyId = options.AppId;//你的accessKeyId，参考本文档步骤2
            String accessKeySecret = options.AppKey;//你的accessKeySecret，参考本文档步骤2
            IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", accessKeyId, accessKeySecret);
            //IAcsClient client = new DefaultAcsClient(profile);
            // SingleSendSmsRequest request = new SingleSendSmsRequest();
            //初始化ascClient,暂时不支持多region（请勿修改）
            profile.AddEndpoint("cn-hangzhou", "cn-hangzhou", product, domain);
            IAcsClient acsClient = new DefaultAcsClient(profile);
            SendSmsRequest request = new SendSmsRequest();
            try
            {
                //必填:待发送手机号码。支持以逗号分隔的形式进行批量调用，批量上限为1000个手机号码,批量调用相对于单条调用及时性稍有延迟,验证码类型的短信推荐使用单条调用的方式，发送国际/港澳台消息时，接收号码格式为00+国际区号+号码，如“0085200000000”
                request.PhoneNumbers = phones.Join(",");
                //必填:短信签名-可在短信控制台中找到
                request.SignName = template.SmsSign;
                //必填:短信模板-可在短信控制台中找到，发送国际/港澳台消息时，请使用国际/港澳台短信模版
                request.TemplateCode = template.TemplateId;
                //可选:模板中的变量替换JSON串,如模板内容为"亲爱的${name},您的验证码为${code}"时,此处的值为
                var psDic = args.Select((item, index) => new KeyValuePair<string, string>($"p{index + 1}", item)).ToDictionary(r => r.Key, r =>
                {
                    // 20个字符以上截断
                    return r.Value.Length > 20 ? r.Value.Substring(0, 17) + "..." : r.Value;
                });
                request.TemplateParam = JsonConvert.SerializeObject(psDic);
                //可选:outId为提供给业务方扩展字段,最终在短信回执消息中将此值带回给调用者
                //request.OutId = "yourOutId";
                //请求失败这里会抛ClientException异常

                SendSmsResponse sendSmsResponse = acsClient.GetAcsResponse(request);
                if (!sendSmsResponse.Code.EqualsIgnoreCase("OK"))
                    throw new SmsDomainException($"阿里云短信发送失败,{sendSmsResponse.Message}:{phones.Join(",")},response error code:{sendSmsResponse.Code}");
            }
            catch (ServerException ex)
            {
                throw new SmsDomainException(ex.Message, ex);
            }
            catch (ClientException ex)
            {
                throw new SmsDomainException(ex.Message, ex);
            }
        }
    }
}