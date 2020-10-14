namespace Shashlik.Response
{
    public enum ResponseStatus
    {
        /// <summary>
        /// 其它
        /// </summary>
        Other,
        
        /// <summary>
        /// 参数错误
        /// </summary>
        ArgError,

        /// <summary>
        /// 操作/逻辑错误
        /// 
        /// </summary>
        LogicalError,

        /// <summary>
        /// 未授权
        /// </summary>
        Unauthorized,

        /// <summary>
        /// 拒绝请求
        /// </summary>
        Forbidden,

        /// <summary>
        /// 资源不存在
        /// </summary>
        NotFound,

        /// <summary>
        /// 系统错误
        /// </summary>
        SystemError
    }
}