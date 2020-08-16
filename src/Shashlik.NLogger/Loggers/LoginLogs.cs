using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shashlik.NLogger.Loggers
{
    /// <summary>
    /// 登录日志
    /// </summary>
    public class LoginLogs
    {
        public long Id { get; set; }

        /// <summary>
        /// 记录人
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// 客户端
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// logger
        /// </summary>
        public string Logger { get; set; }

        /// <summary>
        /// 记录时间
        /// </summary>
        public DateTime LogTime { get; set; }

        /// <summary>
        /// 日志级别
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 客户端ip
        /// </summary>
        public string ClientIp { get; set; }
    }
}
