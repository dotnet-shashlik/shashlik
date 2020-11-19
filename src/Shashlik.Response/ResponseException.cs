using System;

// ReSharper disable MemberCanBePrivate.Global

namespace Shashlik.Response
{
    /// <summary>
    /// 异常响应
    /// </summary>
    public class ResponseException : Exception
    {
        /// <summary>
        /// 调试信息
        /// </summary>
        public string? Debug { get; }

        /// <summary>
        /// 响应状态枚举,ResponseStatus和ErrorCode必有一个有值
        /// </summary>
        public ResponseStatus ResponseStatus { get; }

        /// <summary>
        /// 自定义错误代码,ResponseStatus.Other时有效
        /// </summary>
        public int ErrorCode { get; }

        private ResponseException(string? message, string? debug) : base(message ?? string.Empty)
        {
            Debug = debug;
        }


        /// <summary>
        /// 常用错误异常类
        /// </summary>
        /// <param name="message">错误消息</param>
        /// <param name="responseStatus">错误状态</param>
        /// <param name="debug">调试信息</param>
        public ResponseException(string? message, ResponseStatus responseStatus, string? debug = null)
            : this(message, debug)
        {
            ResponseStatus = responseStatus;
        }

        /// <summary>
        /// 常用错误异常类
        /// </summary>
        /// <param name="message">错误消息</param>
        /// <param name="errorCode">自定义的错误状态码</param>
        /// <param name="debug">调试信息</param>
        public ResponseException(string? message, int errorCode, string? debug = null) : this(message, debug)
        {
            ErrorCode = errorCode;
            ResponseStatus = ResponseStatus.Other;
        }

        /// <summary>
        /// 参数错误异常
        /// </summary>
        /// <param name="msg">错误消息</param>
        /// <param name="debug">调试内容</param>
        /// <returns></returns>
        public static ResponseException ArgError(string? msg = null, string? debug = null)
        {
            return new ResponseException(msg, ResponseStatus.ArgError, debug);
        }

        /// <summary>
        /// 操作错误
        /// </summary>
        /// <param name="msg">错误消息</param>
        /// <param name="debug">调试内容</param>
        /// <returns></returns>
        public static ResponseException LogicalError(string? msg = null, string? debug = null)
        {
            return new ResponseException(msg, ResponseStatus.LogicalError, debug);
        }

        /// <summary>
        /// 系统繁忙
        /// </summary>
        /// <param name="msg">错误消息</param>
        /// <param name="debug">调试内容</param>
        /// <returns></returns>
        public static ResponseException SystemError(string? msg = null, string? debug = null)
        {
            return new ResponseException(msg, ResponseStatus.SystemError, debug);
        }

        /// <summary>
        /// 资源不存在错误
        /// </summary>
        /// <param name="msg">错误消息</param>
        /// <param name="debug">调试内容</param>
        /// <returns></returns>
        public static ResponseException NotFound(string? msg = null, string? debug = null)
        {
            return new ResponseException(msg, ResponseStatus.NotFound, debug);
        }

        /// <summary>
        /// 拒绝访问错误
        /// </summary>
        /// <param name="msg">错误消息</param>
        /// <param name="debug">调试内容</param>
        /// <returns></returns>
        public static ResponseException Forbidden(string? msg = null, string? debug = null)
        {
            return new ResponseException(msg, ResponseStatus.Forbidden, debug);
        }

        /// <summary>
        /// 未认证用户
        /// </summary>
        /// <param name="msg">错误消息</param>
        /// <param name="debug">调试内容</param>
        /// <returns></returns>
        public static ResponseException UnAuthentication(string? msg = null, string? debug = null)
        {
            return new ResponseException(msg, ResponseStatus.UnAuthentication, debug);
        }

        /// <summary>
        /// 其他错误
        /// </summary>
        /// <param name="code">错误代码</param>
        /// <param name="msg">错误消息</param>
        /// <param name="debug">调试内容</param>
        /// <returns></returns>
        public static ResponseException Other(int code, string? msg = null, string? debug = null)
        {
            return new ResponseException(msg, code, debug);
        }
    }
}