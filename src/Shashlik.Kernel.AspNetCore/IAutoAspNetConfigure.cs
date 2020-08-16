﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;

namespace Shashlik.Kernel
{
    /// <summary>
    /// Shashlik aspnet core 配置
    /// </summary>
    public interface IAutoAspNetConfigure
    {
        void Configure(IApplicationBuilder App);
    }
}
