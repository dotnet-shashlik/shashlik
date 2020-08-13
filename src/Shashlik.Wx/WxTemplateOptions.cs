using System;
using System.Collections.Generic;
using System.Text;

namespace Guc.Wx
{

    public class WxTemplateMsgModel
    {
        /// <summary>
        /// 模板代码,自定义的,不区分大小写
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 模板id
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// 模板跳转链接（海外帐号没有跳转能力）
        /// <para>url和miniprogram都是非必填字段，若都不传则模板无跳转；若都传，会优先跳转至小程序。开发者可根据实际需要选择其中一种跳转方式即可。当用户的微信客户端版本不支持跳小程序时，将会跳转至url。</para>
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 所需跳转到的小程序appid（该小程序appid必须与发模板消息的公众号是绑定关联关系，暂不支持小游戏）
        /// </summary>
        public string MiniProgramAppId { get; set; }

        /// <summary>
        /// 所需跳转到小程序的具体页面路径，支持带参数,（示例index?foo=bar），要求该小程序已发布，暂不支持小游戏
        /// </summary>
        public string MiniProgramPagePath { get; set; }

        /// <summary>
        /// 模板数据,支持razor格式化,格式如下:
        /// <para>
        /// 带有颜色:#c9c9c9|尊敬的@{Name}老王,您的认证已经通过!
        /// </para>
        /// <para>
        /// 不带颜色:尊敬的@{Name}老王,您的认证已经通过!
        /// </para>
        /// </summary>
        public List<string> Datas { get; set; }
    }

    /// <summary>
    /// 格式化微信模板消息配置
    /// </summary>
    public class WxTemplateMsgOptions
    {
        /// <summary>
        /// 所有的模板配置
        /// </summary>
        public List<WxTemplateMsgModel> Templates { get; set; }
    }
}
