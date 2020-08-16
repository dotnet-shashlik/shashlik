using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.PushNotification
{
    public class PushNotificationTemplateOptions
    {
        public List<PushNotificationMessage> Templates { get; set; }
    }

    public class PushNotificationMessage
    {
        /// <summary>
        /// 消息分类
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 消息标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 消息图片
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// 模板数据,支持razor格式化,格式如下:
        /// <para>
        /// 带有颜色:#c9c9c9|尊敬的@{Name}老王,您的认证已经通过!
        /// </para>
        /// <para>
        /// 不带颜色:尊敬的@{Name}老王,您的认证已经通过!
        /// </para>
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 附加信息
        /// </summary>
        public Dictionary<string, object> Extras { get; set; }
    }
}
