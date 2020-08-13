using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Guc.NLogger.Loggers
{
    /// <summary>
    /// 操作日志
    /// </summary>
    public class OperationLogs
    {
        public long Id { get; set; }

        /// <summary>
        /// 记录人
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// 客户端id
        /// </summary>
        public string ClientId { get; set; }

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
        /// logger
        /// </summary>
        public string Logger { get; set; }

        /// <summary>
        /// 客户端id
        /// </summary>
        public string ClientIp { get; set; }

        /// <summary>
        /// 请求类型
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        public string RequestUrl { get; set; }

        /// <summary>
        /// 请求体
        /// </summary>
        public string RequestBody { get; set; }

        /// <summary>
        /// url参数
        /// </summary>
        public string RequestQueryString { get; set; }

        /// <summary>
        /// form参数
        /// </summary>
        public string RequestFormData { get; set; }
    }
}
