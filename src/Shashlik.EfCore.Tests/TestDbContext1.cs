﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Shashlik.EfCore.Tests
{
    // dotnet ef migrations add init -c TestDbContext  -o Migrations -p ./Shashlik.EfCore.Tests/Shashlik.EfCore.Tests.csproj -s ./Shashlik.EfCore.Tests/Shashlik.EfCore.Tests.csproj

    [AutoMigration]
    public class TestDbContext1 : ShashlikDbContext<TestDbContext1>
    {
        public TestDbContext1(DbContextOptions<TestDbContext1> options) : base(options)
        {
        }
    }
}