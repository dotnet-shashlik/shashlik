using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Guc.Kernel.Exception
{
    public class GucException : System.Exception
    {
        /// <summary>
        /// 日志内容
        /// </summary>
        public string LogContent { get; }

        /// <summary>
        /// 调试内容
        /// </summary>
        public string Debug { get; }

        /// <summary>
        /// 错误代码
        /// </summary>
        public int Code { get; }

        /// <summary>
        /// GucException
        /// </summary>
        /// <param name="message">错误消息</param>
        /// <param name="code">错误代码,默认是参数错误</param>
        /// <param name="logContent">记录到日志系统的内容,null不记录</param>
        public GucException(string message, int? code = null, string logContent = null, string debug = null) : base(message ?? "")
        {
            Code = code ?? ExceptionCodes.Instance.ArgError;
            LogContent = logContent;
            Debug = debug;
        }

        /// <summary>
        /// 参数错误异常
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static GucException ArgError(string msg = null, string debug = null)
        {
            return new Guc.Kernel.Exception.GucException(msg, ExceptionCodes.Instance.ArgError, debug: debug);
        }

        /// <summary>
        /// 操作错误
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static GucException LogicalError(string msg = null, string debug = null)
        {
            return new Guc.Kernel.Exception.GucException(msg, ExceptionCodes.Instance.LogicalError, debug: debug);
        }

        /// <summary>
        /// 系统繁忙
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static GucException Busy(string msg = null, string debug = null)
        {
            return new Guc.Kernel.Exception.GucException(msg, ExceptionCodes.Instance.LogicalError, debug: debug);
        }

        /// <summary>
        /// 没找到404
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static GucException NotFound(string msg = null, string debug = null)
        {
            return new Guc.Kernel.Exception.GucException(msg, ExceptionCodes.Instance.NotFound, debug: debug);
        }

        /// <summary>
        /// 403
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static GucException Forbid(string msg = null, string debug = null)
        {
            return new Guc.Kernel.Exception.GucException(msg, ExceptionCodes.Instance.Forbid, debug: debug);
        }

        /// <summary>
        /// 401
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static GucException UnAuth(string msg = null, string debug = null)
        {
            return new Guc.Kernel.Exception.GucException(msg, ExceptionCodes.Instance.UnAuth, debug: debug);
        }
    }
}
