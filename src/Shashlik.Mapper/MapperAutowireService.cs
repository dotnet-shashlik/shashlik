using System.Linq;
using System.Reflection;
using AutoMapper;
using System;
using AutoMapper.QueryableExtensions;
using Shashlik.Kernel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System.Linq.Expressions;
using Shashlik.Utils.Extensions;
using System.Collections.Generic;
using Shashlik.Utils.Common;
using Shashlik.Kernel.Autowire;
using Microsoft.Extensions.Configuration;

namespace Shashlik.Mapper
{
    public class MapperAutowireService : IAutowireConfigureServices
    {
        public void ConfigureServices(IKernelService kernelService, IConfiguration rootConfiguration)
        {
            kernelService.AddAutoMapperByConvention();
        }
    }
}