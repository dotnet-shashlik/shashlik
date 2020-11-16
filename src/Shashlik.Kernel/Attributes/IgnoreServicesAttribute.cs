using System;

namespace Shashlik.Kernel.Attributes
{
    public class IgnoreServicesAttribute : Attribute
    {
        /// <summary>
        /// 将忽略的服务
        /// </summary>
        /// <param name="ignoreServices"></param>
        public IgnoreServicesAttribute(params Type[] ignoreServices)
        {
            IgnoreServices = ignoreServices;
        }

        public Type[] IgnoreServices { get; }
    }
}