using System;

namespace Shashlik.Kernel.Attributes
{
    public class ExternalServicesAttribute : Attribute
    {
        /// <summary>
        /// 额外注册的服务
        /// </summary>
        /// <param name="externalServices"></param>
        public ExternalServicesAttribute(params Type[] externalServices)
        {
            ExternalServices = externalServices;
        }

        public Type[] ExternalServices { get; }
    }
}