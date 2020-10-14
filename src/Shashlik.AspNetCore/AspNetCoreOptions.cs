using Shashlik.Kernel.Autowired.Attributes;

namespace Shashlik.AspNetCore
{
    [AutoOptions("Shashlik.AspNetCore")]
    public class AspNetCoreOptions
    {
        /// <summary>
        /// 是否输出debug信息
        /// </summary>
        public bool IsDebug { get; set; } = true;

        /// <summary>
        /// 响应异常转换为对应的http状态码,默认false
        /// </summary>
        public bool UseResponseExceptionToHttpCode { get; set; } = false;

        /// <summary>
        /// 是否返回所有的模型验证错误,默认false,返回第一个错误. 返回所有的错误时,多条错误信息以\n分割
        /// </summary>
        public bool ResponseAllModelError { get; set; } = false;

        /// <summary>
        /// 返回状态码配置
        /// </summary>
        public ResponseCode ResponseCode { get; set; } = new ResponseCode();
    }
}