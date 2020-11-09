using System.Collections.Generic;
using Shashlik.Kernel.Dependency;
using Shashlik.Sms.Options;

namespace Shashlik.Sms
{
    /// <summary>
    /// 短信发送执行接口
    /// </summary>
    public interface ISmsDomain : ISingleton
    {
        /// <summary>
        /// 主机类型,aliyun/tencent ....
        /// </summary>
        string SmsDomain { get; }

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="options"></param>
        /// <param name="phones"></param>
        /// <param name="subject"></param>
        /// <param name="args"></param>
        void Send(SmsDomainConfig options, IEnumerable<string> phones, string subject, params string[] args);
    }
}