using Shashlik.Utils.Extensions;
using ResponseStatus = Shashlik.Response.ResponseStatus;

// ReSharper disable MemberCanBePrivate.Global

namespace Shashlik.AspNetCore
{
    public class ResponseCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        public int Success { get; set; } = 200;

        /// <summary>
        /// 成功
        /// </summary>
        public string SuccessDefaultMessage { get; set; } = "success";

        /// <summary>
        /// 参数错误
        /// </summary>
        public int ArgError { get; set; } = 400;

        /// <summary>
        /// 参数错误
        /// </summary>
        public string ArgErrorDefaultMessage { get; set; } = "argument error";

        /// <summary>
        /// 操作/逻辑错误
        /// </summary>
        public int LogicalError { get; set; } = 409;

        /// <summary>
        /// 操作/逻辑错误
        /// </summary>
        public string LogicalErrorDefaultMessage { get; set; } = "logical error";

        /// <summary>
        /// 未授权
        /// </summary>
        public int UnAuthentication { get; set; } = 401;

        /// <summary>
        /// 未授权
        /// </summary>
        public string UnAuthenticationDefaultMessage { get; set; } = "no authentication";

        /// <summary>
        /// 拒绝请求
        /// </summary>
        public int Forbidden { get; set; } = 403;

        /// <summary>
        /// 拒绝请求
        /// </summary>
        public string ForbiddenDefaultMessage { get; set; } = "forbidden";

        /// <summary>
        /// 资源不存在
        /// </summary>
        public int NotFound { get; set; } = 404;

        /// <summary>
        /// 资源不存在
        /// </summary>
        public string NotFoundDefaultMessage { get; set; } = "not found";

        /// <summary>
        /// 系统错误
        /// </summary>
        public int SystemError { get; set; } = 500;

        /// <summary>
        /// 系统错误
        /// </summary>
        public string SystemErrorDefaultMessage { get; set; } = "system error";

        /// <summary>
        /// 状态枚举转换位响应状态码
        /// </summary>
        /// <param name="status"></param>
        /// <param name="otherErrorCode"></param>
        /// <returns></returns>
        public int FormatCode(ResponseStatus status, int otherErrorCode)
        {
            return status switch
            {
                ResponseStatus.ArgError => ArgError,
                ResponseStatus.LogicalError => LogicalError,
                ResponseStatus.UnAuthentication => UnAuthentication,
                ResponseStatus.Forbidden => Forbidden,
                ResponseStatus.NotFound => NotFound,
                ResponseStatus.SystemError => SystemError,
                ResponseStatus.Other => otherErrorCode,
                _ => (int) status
            };
        }

        /// <summary>
        /// 状态枚举转换位响应状态码
        /// </summary>
        /// <param name="status"></param>
        /// <param name="inputMessage">`</param>
        /// <returns></returns>
        public string FormatMessage(ResponseStatus status, string inputMessage)
        {
            return status switch
            {
                ResponseStatus.ArgError => inputMessage.IsNullOrWhiteSpace()
                    ? ArgErrorDefaultMessage
                    : inputMessage,
                ResponseStatus.LogicalError => inputMessage.IsNullOrWhiteSpace()
                    ? LogicalErrorDefaultMessage
                    : inputMessage,
                ResponseStatus.UnAuthentication => inputMessage.IsNullOrWhiteSpace()
                    ? UnAuthenticationDefaultMessage
                    : inputMessage,
                ResponseStatus.Forbidden => inputMessage.IsNullOrWhiteSpace()
                    ? ForbiddenDefaultMessage
                    : inputMessage,
                ResponseStatus.NotFound => inputMessage.IsNullOrWhiteSpace()
                    ? NotFoundDefaultMessage
                    : inputMessage,
                ResponseStatus.SystemError => inputMessage.IsNullOrWhiteSpace()
                    ? SystemErrorDefaultMessage
                    : inputMessage,
                ResponseStatus.Other => inputMessage,
                _ => inputMessage
            };
        }
    }
}