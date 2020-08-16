using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Shashlik.Utils.Common;
using Shashlik.Utils.Extensions;
using Jiguang.JPush;
using Jiguang.JPush.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Shashlik.PushNotification
{
    public class JiGuangNotificationPusher : INotificationPusher, Shashlik.Kernel.Dependency.ISingleton
    {
        private JPushClient JPushClient { get; }
        private ILogger<JiGuangNotificationPusher> Logger { get; }
        private IHostEnvironment Environment { get; }

        public JiGuangNotificationPusher(JPushClient jPushClient, ILogger<JiGuangNotificationPusher> logger, IHostEnvironment environment)
        {
            JPushClient = jPushClient;
            JPushClient.SetBaseURL(JPushClient.BASE_URL_PUSH_BEIJING);
            Logger = logger;
            Environment = environment;
        }

        private string UserIdPrefix => Environment.IsProduction() ? "prod_" : "dev_";

        public async Task PushAsync(string userId, string category, string content, string title = null, string image = null,
            Dictionary<string, object> extras = null)
        {
            userId = UserIdPrefix + userId;
            var pushPayload = new PushPayload()
            {
                Platform = "all",
                Audience = new { alias = new List<string> { userId } },
                Notification = new Notification
                {
                    Alert = content,
                    Android = new Android
                    {
                        Alert = content,
                        Title = title,
                        LargeIcon = image,
                        Extras = extras
                    },
                    IOS = new IOS
                    {
                        Alert = new Dictionary<string, string>()
                        {
                            {"title", title },
                            {"body", content },
                            {"launch-image", image }
                        },
                        Extras = extras
                    }
                },
                Options = new Options
                {
                    IsApnsProduction = !Environment.IsDevelopment() // 设置 iOS 推送生产环境。不设置默认为开发环境。
                }
            };
            var response = await JPushClient.SendPushAsync(pushPayload);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Logger.LogError($"极光推送失败user:{userId}, category:{category}, response:{response.StatusCode}，返回数据{response.Content}");
            }
        }

        public async Task<string> SchedulePushAsync(DateTime pushTime, string userId, string category, string content,
            string title = null, string image = null, Dictionary<string, object> extras = null)
        {
            userId = UserIdPrefix + userId;
            var pushPayload = new PushPayload()
            {
                Platform = "all",
                Audience = new { alias = new List<string> { userId } },
                Notification = new Notification
                {
                    Alert = content,
                    Android = new Android
                    {
                        Alert = content,
                        Title = title,
                        LargeIcon = image,
                        Extras = extras
                    },
                    IOS = new IOS
                    {
                        Alert = new Dictionary<string, string>()
                        {
                            {"title", title },
                            {"body", content },
                            {"launch-image", image }
                        },
                        Extras = extras
                    }
                },
                Options = new Options
                {
                    IsApnsProduction = !Environment.IsDevelopment() // 设置 iOS 推送生产环境。不设置默认为开发环境。
                }
            };

            var response = await JPushClient.Schedule.CreateSingleScheduleTaskAsync("定时推送", pushPayload, pushTime.ToStringyyyyMMddHHmmss());
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Logger.LogError($"极光推送失败user:{userId}, category:{category}, response:{response.StatusCode}，返回数据{response.Content}");
                return null;
            }

            var jObject = JsonHelper.Deserialize<JObject>(response.Content);
            if (jObject == null)
            {
                Logger.LogError($"处理极光推送返回数据失败，返回数据{response.Content}");
                return null;
            }

            return jObject["schedule_id"]?.ToString();
        }

        public async Task CancelSchedulePush(string scheduleId)
        {
            var response = await JPushClient.Schedule.DeleteScheduleTaskAsync(scheduleId);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Logger.LogError($"删除定时推送失败:{response.StatusCode}，返回数据{response.Content}");
            }
        }
    }
}
