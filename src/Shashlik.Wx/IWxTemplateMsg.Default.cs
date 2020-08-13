using Senparc.Weixin.MP.AdvancedAPIs;
using System;
using System.Collections.Generic;
using System.Text;
using Senparc.Weixin;
using Senparc.Weixin.MP.Containers;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Guc.Utils.Extensions;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;

namespace Guc.Wx
{
    /// <summary>
    /// 微信模板消息
    /// </summary>
    public class DefaultWxTemplateMsg : IWxTemplateMsg
    {
        public DefaultWxTemplateMsg(
            IWxSettings wxSettings,
            IOptionsSnapshot<WxTemplateMsgOptions> templateOptions
            )
        {
            this.wxSettings = wxSettings;
            this.templateOptions = templateOptions;
        }

        IWxSettings wxSettings { get; }
        IOptionsSnapshot<WxTemplateMsgOptions> templateOptions { get; }

        public void SendWithCode(string openid, string templateCode, object data)
        {
            Send(openid, templateCode, data, (value, d) => value.RazorFormat(d));
        }

        private void Send<T>(string openid, string templateCode, T data, Func<string, T, string> format)
        {
            var model = templateOptions.Value.Templates.FirstOrDefault(r => r.Code.EqualsIgnoreCase(templateCode));
            if (model == null)
                throw new Exception($"找不到微信消息模板:{templateCode}");
            if (model.TemplateId.IsNullOrWhiteSpace())
                // 模板id 为空则不发送
                return;

            var keys = templateDic.GetOrDefault(model.TemplateId);
            if (keys.IsNullOrEmpty())
            {
                // 清除,重新加载
                templateDic.Clear();
                var res = TemplateApi.GetPrivateTemplateAsync(wxSettings.Setting.WeixinAppId).GetAwaiter().GetResult();
                var reg = new Regex(@"\{\{[^\{\}]{1,}.DATA\}\}");
                // 正则匹配查找模板中的键
                foreach (var item in res.template_list)
                {
                    var matches = reg.Matches(item.content);

                    var list = new List<string>();
                    foreach (Match math in matches)
                    {
                        if (!math.Success)
                            continue;
                        var v = math.Value.AsSpan().Slice(2);
                        v = v.Slice(0, v.Length - 7);
                        list.Add(v.ToString());
                    }

                    if (list.Count == 0)
                        throw new Exception($"模板消息:{item.title},内容错误:{item.content}");

                    templateDic.TryAdd(item.template_id, list);
                }
                keys = templateDic.GetOrDefault(model.TemplateId);
            }

            if (keys.IsNullOrEmpty())
                throw new Exception($"错误的模板消息配置,code:{templateCode},templateId:{model.TemplateId}");

            var token = wxSettings.Token;

            var colorReg = new Regex("^#([0-9a-fA-F]{6}|[0-9a-fA-F]{3})$");
            var dataDic = new Dictionary<string, object>();
            for (int i = 0; i < keys.Count; i++)
            {
                string value = "";
                string color = "";
                if (model.Datas.Count >= i + 1)
                {
                    value = model.Datas[i];
                    var span = value.AsSpan();
                    if (span.StartsWith(new[] { '#' }) && span[7] == '|')
                    {
                        color = span.Slice(0, 7).ToString();
                        if (!colorReg.IsMatch(color))
                            color = "";
                        value = span.Slice(8).ToString();
                    }
                }
                if (data != null && !value.IsNullOrWhiteSpace())
                    value = format(value, data);
                dataDic.Add(keys[i], new { value, color });
            }
            var url = model.Url;
            if (!url.IsNullOrWhiteSpace())
                url = format(url, data);
            if (!string.IsNullOrEmpty(model.MiniProgramPagePath))
            {
                model.MiniProgramPagePath = format(model.MiniProgramPagePath, data);
            }
            TemplateApi.SendTemplateMessageAsync(token, openid, model.TemplateId, url, dataDic, new TempleteModel_MiniProgram
            {
                appid = model.MiniProgramAppId,
                pagepath = model.MiniProgramPagePath
            });
        }

        /// <summary>
        /// 缓存所有的模板列表数据,key为templateId,value为keys:first,keyword1,keyword2,remark
        /// </summary>
        static ConcurrentDictionary<string, List<string>> templateDic { get; } = new ConcurrentDictionary<string, List<string>>();
    }
}