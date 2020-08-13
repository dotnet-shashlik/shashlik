using Guc.Kernel;
using Guc.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Guc.AspNetCore
{
    /// <summary>
    /// 响应结果
    /// </summary>
    public class ResponseResult
    {
        /// <summary>
        /// 错误代码,1:正常,其他的为业务错误代码
        /// </summary>
        public int Code { get; set; } = Guc.Kernel.Exception.ExceptionCodes.Instance.Success;

        /// <summary>
        /// 错误消息
        /// </summary>
        public string Msg { get; set; } = "操作成功";

        /// <summary>
        /// 数据
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 调试信息
        /// </summary>
        public string Debug { get; set; }

        public ResponseResult(object data)
        {
            this.Data = data;
        }

        public ResponseResult(string errMsg, int code, object data = null) : this(data)
        {
            this.Code = code;
            this.Msg = errMsg;
        }

        public ResponseResult()
        {
        }
    }
}
