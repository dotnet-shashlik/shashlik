using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.Validation
{
    /// <summary>
    /// 内部服务
    /// </summary>
    class InternalServices
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public static void Init(IServiceProvider serviceProvider)
        {
            if (ServiceProvider != null)
                throw new Exception("internal services has been inited.");
            ServiceProvider = serviceProvider;
        }
    }
}
