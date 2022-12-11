using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel.Dependency;
using Shashlik.Kernel.Test.Autowired;

namespace Shashlik.EfCore.Tests;

[Transient]
public class Migration : ITestAutowiredConfigure
{
    public void Configure(IServiceProvider serviceProvider)
    {
        serviceProvider.GetRequiredService<TestDbContext1>().Database.Migrate();
    }
}