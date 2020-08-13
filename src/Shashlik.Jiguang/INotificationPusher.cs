using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Guc.PushNotification
{
    /// <summary>
    /// app消息推送接口
    /// </summary>
    public interface INotificationPusher
    {
        /// <summary>
        /// 推送消息
        /// </summary>
        /// <param name="userId">接收用户ID</param>
        /// <param name="category">消息分类</param>
        /// <param name="content">消息内容</param>
        /// <param name="title">消息标题</param>
        /// <param name="image">消息图片</param>
        /// <param name="extras">附加字段</param>
        /// <returns></returns>
        Task PushAsync(string userId, string category, string content, string title = null, string image = null,
            Dictionary<string, object> extras = null);

        /// <summary>
        /// 定时推送消息
        /// </summary>
        /// <param name="pushTime">推送时间</param>
        /// <param name="userId">接收用户ID</param>
        /// <param name="category">消息分类</param>
        /// <param name="content">消息内容</param>
        /// <param name="title">消息标题</param>
        /// <param name="image">消息图片</param>
        /// <param name="extras">附加字段</param>
        /// <returns></returns>
        Task<string> SchedulePushAsync(DateTime pushTime, string userId, string category, string content,
            string title = null, string image = null, Dictionary<string, object> extras = null);

        /// <summary>
        /// 删除定时推送
        /// </summary>
        /// <param name="scheduleId"></param>
        /// <returns></returns>
        Task CancelSchedulePush(string scheduleId);
    }
}
