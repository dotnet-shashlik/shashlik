using Shashlik.Kernel.Attributes;

namespace Shashlik.AspNetCore
{
    [AutoOptions("Shashlik.AspNetCore")]
    public class AspNetCoreOptions
    {
        /// <summary>
        /// 是否输出debug信息
        /// </summary>
        public bool IsDebug { get; set; } = false;

        /// <summary>
        /// 返回状态码配置
        /// </summary>
        public ResponseCode ResponseCode { get; set; } = new ResponseCode();
    }
}