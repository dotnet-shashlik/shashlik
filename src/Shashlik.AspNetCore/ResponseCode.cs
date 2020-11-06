using System;
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
        public int Unauthorized { get; set; } = 401;

        /// <summary>
        /// 未授权
        /// </summary>
        public string UnauthorizedDefaultMessage { get; set; } = "unauthorized";

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
        public int GetCode(Response.ResponseStatus status, int otherErrorCode)
        {
            return status switch
            {
                ResponseStatus.ArgError => this.ArgError,
                ResponseStatus.LogicalError => this.LogicalError,
                ResponseStatus.Unauthorized => this.Unauthorized,
                ResponseStatus.Forbidden => this.Forbidden,
                ResponseStatus.NotFound => this.NotFound,
                ResponseStatus.SystemError => this.SystemError,
                ResponseStatus.Other => otherErrorCode,
                _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
            };
        }
    }
}