using System;
using System.Collections.Generic;
using System.Text;

namespace Guc.Kernel.Exception
{
    /// <summary>
    /// 异常状态码
    /// </summary>
    public class ExceptionCodes
    {
        /// <summary>
        /// 结果状态码
        /// </summary>
        /// <param name="success">成功</param>
        /// <param name="argError">参数错误</param>
        /// <param name="logicalError">逻辑错误</param>
        /// <param name="busy">系统繁忙</param>
        /// <param name="unAuthenticate">未认证</param>
        /// <param name="unAuthorize">未授权</param>
        /// <param name="notFound">资源不存在</param>
        public ExceptionCodes(
            int success = 1,
            int argError = 0,
            int logicalError = -1,
            int busy = -2,
            int unAuthenticate = 401,
            int unAuthorize = 403,
            int notFound = 404
            )
        {
            Success = success;
            ArgError = argError;
            LogicalError = logicalError;
            Busy = busy;
            NotFound = notFound;
            Forbid = unAuthorize;
            UnAuth = unAuthenticate;
        }

        /// <summary>
        /// 静态实例
        /// </summary>
        public static ExceptionCodes Instance { get; internal set; }

        /// <summary>
        /// 成功
        /// </summary>
        public int Success { get; } = 1;

        /// <summary>
        /// 参数错误
        /// </summary>
        public int ArgError { get; } = 0;

        /// <summary>
        /// 操作错误
        /// </summary>
        public int LogicalError { get; } = -1;

        /// <summary>
        /// 系统繁忙
        /// </summary>
        public int Busy { get; } = -2;

        /// <summary>
        /// 未认证
        /// </summary>
        public int UnAuth { get; } = 401;

        /// <summary>
        /// 没权限
        /// </summary>
        public int Forbid { get; } = 403;

        /// <summary>
        /// 找不到
        /// </summary>
        public int NotFound { get; } = 404;
    }
}
