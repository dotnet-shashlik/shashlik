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
        /// 是否写入日志
        /// </summary>
        public bool WriteLog { get; }

        /// <summary>
        /// 写入日志的内容
        /// </summary>
        public string WriteLogMessage { get; }

        /// <summary>
        /// 调试信息
        /// </summary>
        public string Debug { get; }

        /// <summary>
        /// 响应状态枚举,ResponseStatus和ErrorCode必有一个有值
        /// </summary>
        public ResponseStatus? ResponseStatus { get; }

        /// <summary>
        /// 自定义错误代码,ResponseStatus和ErrorCode必有一个有值
        /// </summary>
        public int ErrorCode { get; }

        private ResponseException(string message, bool writeLog, string writeLogMessage, string debug) :
            base(writeLog ? writeLogMessage ?? message : message)
        {
            WriteLog = writeLog;
            WriteLogMessage = writeLogMessage;
            Debug = debug;
        }


        /// <summary>
        /// 常用错误异常类
        /// </summary>
        /// <param name="message">错误消息</param>
        /// <param name="responseStatus">错误状态</param>
        /// <param name="writeLog">是否记录日志</param>
        /// <param name="writeLogMessage">日志记录内容</param>
        /// <param name="debug">调试信息</param>
        public ResponseException(string message, ResponseStatus responseStatus, bool writeLog, string writeLogMessage,
            string debug) :
            this(message, writeLog, writeLogMessage, debug)
        {
            ResponseStatus = responseStatus;
        }

        /// <summary>
        /// 常用错误异常类
        /// </summary>
        /// <param name="message">错误消息</param>
        /// <param name="errorCode">自定义的错误状态码</param>
        /// <param name="writeLog">是否记录日志</param>
        /// <param name="writeLogMessage">日志记录内容</param>
        /// <param name="debug">调试信息</param>
        public ResponseException(string message, int errorCode, bool writeLog, string writeLogMessage,
            string debug) :
            this(message, writeLog, writeLogMessage, debug)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// 参数错误异常
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static ResponseException ArgError(string msg = null, string debug = null)
        {
            return new ResponseException(msg, Response.ResponseStatus.ArgError, false, null, debug);
        }

        /// <summary>
        /// 操作错误
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static ResponseException LogicalError(string msg = null, string debug = null)
        {
            return new ResponseException(msg, Response.ResponseStatus.LogicalError, false, null, debug);
        }

        /// <summary>
        /// 系统繁忙
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static ResponseException SystemError(string msg = null, string debug = null)
        {
            return new ResponseException(msg, Response.ResponseStatus.SystemError, false, null, debug);
        }

        /// <summary>
        /// 资源不存在错误
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static ResponseException NotFound(string msg = null, string debug = null)
        {
            return new ResponseException(msg, Response.ResponseStatus.NotFound, false, null, debug);
        }

        /// <summary>
        /// 拒绝访问错误
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static ResponseException Forbidden(string msg = null, string debug = null)
        {
            return new ResponseException(msg, Response.ResponseStatus.Forbidden, false, null, debug);
        }

        /// <summary>
        /// 未授权错误
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static ResponseException Unauthorized(string msg = null, string debug = null)
        {
            return new ResponseException(msg, Response.ResponseStatus.Unauthorized, false, null, debug);
        }
    }
}