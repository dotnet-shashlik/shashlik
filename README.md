# Shashlik

[![build and test](https://github.com/dotnet-shashlik/shashlik/workflows/build%20and%20test/badge.svg)](https://github.com/dotnet-shashlik/shashlik)
[![license](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/dotnet-shashlik/shashlik/blob/main/LICENSE)

---

Shashlik，.NET快速集成基础框架，旨在通过配置文件、约定等方式做到简化各种代码配置、模块配置、服务配置，提高程序集的可复用性。现已集成`AutoMapper`、`Sms`、`Identity`、`Identity Server4`、`Cap`、`Captcha`、`Redis`等常用框架和功能，只需要导入相应的程序集，添加对应的配置文件，几乎无需做任何代码改动，做到开箱即用。

Shashlik.Kernel是整个Shashlik的核心程序集，几乎所有的Shashlik组件都基于Shashlik.Kernel进行构建。其包含了约定式服务注册、配置装配、自动装配等核心功能。

## Nuget

| PackageName                         | Nuget                                                                                                                                    | Description                                        |
| ----------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------- |
| Shashlik.Utils                   | [![nuGet](https://img.shields.io/nuget/v/Shashlik.Utils.svg)](https://www.nuget.org/packages/Shashlik.Utils)                   | Shashlik框架基本工具库 |
| Shashlik.Kernel                     | [![nuGet](https://img.shields.io/nuget/v/Shashlik.Kernel.svg)](https://www.nuget.org/packages/Shashlik.Kernel)          | Shashlik框架核心                                           |
| Shashlik.AspNetCore                   | [![nuGet](https://img.shields.io/nuget/v/Shashlik.AspNetCore.svg)](https://www.nuget.org/packages/Shashlik.AspNetCore)                   | AspNetCore功能集成，封装异常、统一结果等功能 |
| Shashlik.AutoMapper             | [![nuGet](https://img.shields.io/nuget/v/Shashlik.AutoMapper.svg)](https://www.nuget.org/packages/Shashlik.AutoMapper)             | AutoMapper功能集成，简化AutoMapper映射配置和使用                                 |
| Shashlik.Cap        | [![nuGet](https://img.shields.io/nuget/v/Shashlik.Cap.svg)](https://www.nuget.org/packages/Shashlik.Cap)        | [CAP](https://github.com/dotnetcore/CAP)框架基于事件总线机制集成，简化事件的发布与订阅                            |
| Shashlik.Cap.Rabbit        | [![nuGet](https://img.shields.io/nuget/v/Shashlik.Cap.Rabbit.svg)](https://www.nuget.org/packages/Shashlik.Cap.Rabbit)        | CAP框架RabbitMQ消息中间件的简单集成                           |
| Shashlik.Captcha         | [![nuGet](https://img.shields.io/nuget/v/Shashlik.Captcha.svg)](https://www.nuget.org/packages/Shashlik.Captcha)         | 通用验证码接口抽象                             |
| Shashlik.Captcha.DistributedCache             | [![nuGet](https://img.shields.io/nuget/v/Shashlik.Captcha.DistributedCache.svg)](https://www.nuget.org/packages/Shashlik.Captcha.DistributedCache)             | 验证码的分布式缓存实现                                 |
| Shashlik.Captcha.Redis             | [![nuGet](https://img.shields.io/nuget/v/Shashlik.Captcha.Redis.svg)](https://www.nuget.org/packages/Shashlik.Captcha.Redis)             | 验证码的Redis实现                                 |
| Shashlik.Captcha.Totp             | [![nuGet](https://img.shields.io/nuget/v/Shashlik.Captcha.Totp.svg)](https://www.nuget.org/packages/Shashlik.Captcha.Totp)             | 验证码的Totp实现                                 |
| Shashlik.DataProtection.MySql          | [![nuGet](https://img.shields.io/nuget/v/Shashlik.DataProtection.MySql.svg)](https://www.nuget.org/packages/Shashlik.DataProtection.MySql)          | DataProtection的MySql密钥储存实现                              |
| Shashlik.DataProtection.PostgreSql          | [![nuGet](https://img.shields.io/nuget/v/Shashlik.DataProtection.PostgreSql.svg)](https://www.nuget.org/packages/Shashlik.DataProtection.PostgreSql)          | DataProtection的PostgreSql密钥储存实现                              |
| Shashlik.DataProtection.Redis          | [![nuGet](https://img.shields.io/nuget/v/Shashlik.DataProtection.Redis.svg)](https://www.nuget.org/packages/Shashlik.DataProtection.Redis)          | DataProtection的Redis密钥储存实现（基于CSRedisCore）                              |
| Shashlik.EfCore | [![nuGet](https://img.shields.io/nuget/v/Shashlik.EfCore.svg)](https://www.nuget.org/packages/Shashlik.EfCore) | EfCore常用功能集成：软删除、嵌套事务、自动迁移、约定实体注册、FluentApi自动注册等                                 |
| Shashlik.EfCore.Transactional | [![nuGet](https://img.shields.io/nuget/v/Shashlik.EfCore.Transactional.svg)](https://www.nuget.org/packages/Shashlik.EfCore.Transactional) | EfCore特性事务集成，基于[AspectCore](https://github.com/dotnetcore/AspectCore-Framework)      |
| Shashlik.Identity       | [![nuGet](https://img.shields.io/nuget/v/Shashlik.Identity.svg)](https://www.nuget.org/packages/Identity)       | AspNetCore.Identity集成，                                       |
| Shashlik.Identity.Int32       | [![nuGet](https://img.shields.io/nuget/v/Shashlik.Identity.Int32.svg)](https://www.nuget.org/packages/Shashlik.Identity.Int32)       | Identity，efcore存储，Int32主键类型封装                                       |
| Shashlik.Identity.Int32.MySql       | [![nuGet](https://img.shields.io/nuget/v/Shashlik.Identity.Int32.MySql.svg)](https://www.nuget.org/packages/Shashlik.Identity.Int32.MySql)       | Identity，EfCore存储，Int32主键类型，MySql数据库简单集成                                       |
| Shashlik.Identity.Int32.PostgreSql       | [![nuGet](https://img.shields.io/nuget/v/Shashlik.Identity.Int32.PostgreSql.svg)](https://www.nuget.org/packages/Shashlik.Identity.Int32.PostgreSql)       | Identity，EfCore存储，Int32主键类型，PostgreSql数据库简单集成                                       |
| Shashlik.Ids4       | [![nuGet](https://img.shields.io/nuget/v/Shashlik.Ids4.svg)](https://www.nuget.org/packages/Shashlik.Ids4)       | IdentityServer4集成                                       |
| Shashlik.Ids4.Identity       | [![nuGet](https://img.shields.io/nuget/v/Shashlik.Ids4.Identity.svg)](https://www.nuget.org/packages/Shashlik.Ids4.Identity)       | IdentityServer4使用AspNetCore.Identity集成                                       |
| Shashlik.Ids4.MySqlStore       | [![nuGet](https://img.shields.io/nuget/v/Shashlik.Ids4.MySqlStore.svg)](https://www.nuget.org/packages/Shashlik.Ids4.MySqlStore)       | IdentityServer4使用MySql存储集成                                       |
| Shashlik.Ids4.PostgreSqlStore       | [![nuGet](https://img.shields.io/nuget/v/Shashlik.Ids4.PostgreSqlStore.svg)](https://www.nuget.org/packages/Shashlik.Ids4.PostgreSqlStore)       | IdentityServer4使用MySql存储集成                                      |
| Shashlik.RazorFormat             | [![nuGet](https://img.shields.io/nuget/v/Shashlik.RazorFormat.svg)](https://www.nuget.org/packages/Shashlik.RazorFormat)             | 字符串模板格式化工具库                                 |
| Shashlik.Redis             | [![nuGet](https://img.shields.io/nuget/v/Shashlik.Redis.svg)](https://www.nuget.org/packages/Shashlik.Redis)             | Redis集成，扩展了Redis分布式锁、基于Redis的雪花算法Id生成器                                 |
| Shashlik.Response             | [![nuGet](https://img.shields.io/nuget/v/Shashlik.Response.svg)](https://www.nuget.org/packages/Shashlik.Response)             | 统一结果输出、异常式输出，常用于AspNetCore                                 |
| Shashlik.Sms             | [![nuGet](https://img.shields.io/nuget/v/Shashlik.Sms.svg)](https://www.nuget.org/packages/Shashlik.Sms)             | 短信工具库，已集成阿里云、腾讯云                                 |
| Shashlik.Sms.Cap             | [![nuGet](https://img.shields.io/nuget/v/Shashlik.Sms.Cap.svg)](https://www.nuget.org/packages/Shashlik.Sms.Cap)             | 基于Cap方式的异步短信发送                                 |
| Shashlik.Sms.Limit.DistributedCache             | [![nuGet](https://img.shields.io/nuget/v/Shashlik.Sms.Limit.DistributedCache.svg)](https://www.nuget.org/packages/Shashlik.Sms.Limit.DistributedCache)             | 短信频率限制基于分布式缓存的实现                                 |
| Shashlik.Sms.Limit.Redis             | [![nuGet](https://img.shields.io/nuget/v/Shashlik.Sms.Cap.svg)](https://www.nuget.org/packages/Shashlik.Sms.Cap)             | 短信频率限制基于Redis的实现                                 |

## WIKI
更多配置文档详见[WIKI](https://github.com/dotnet-shashlik/shashlik/wiki)。

## Getting Started

让我们来开发一个使用高德地图来处理地理位置逆信息的功能吧。

1. 安装Shashlik.Kernel

```
    Install-Package Shashlik.Kernel
```

2. 定义配置类
```c#

// AutoOptions：自动装载Shashlik:AMap节点的配置数据，可以自动转换.为:
[AutoOptions("Shashlik.AMap")]
public class AMapOptions
{
    /// <summary>
    /// 高德key
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// 高德api基础地址
    /// </summary>  
    public string BaseUrl{ get; set; }
         
}

```

3. 定义接口
```c#

// Singleton：约定注册IAMap接口和实现
[Singleton]
public interface IAMap
{
    (decimal lat, decimal lng) GetLocation(string address);
}

```

4. 接口实现
```c#
public class DfaultAMap
{
    public DfaultAMap(IOptionsMonitor<AMapOptions> options)
    {
        Options = options;
    }

    private IOptionsMonitor<AMapOptions> Options { get; }

    public (decimal lat, decimal lng) GetLocation(string address)
    {
        // 逻辑代码
    }
}


```

是的，就这样完了，一个可复用的地理信息逆编码功能库就完成了，简化了大量的配置处理（搭配[Shashlik.RC](https://github.com/dotnet-shashlik/rcconfig)远程配置管理，可以爽翻天哦）、服务注册代码。

使用：

1. 定义配置文件
```yaml

Shashlik:
  AMap:
    Key: <your key>
    BaseUrl: https://restapi.amap.com/v3/

```

2. 启用Shashlik
```c#

public void ConfigureServices(IServiceCollection services)
{
    // ....其他服务注册代码

    // 使用Shashlik
    service.AddShashlik();
}

```

3. 调用接口
```c#
public class TestController: Controller
{
    public TestController(IAMap amap)
    {
        AMap = amap;
    } 

    private IAMap AMap { get; }

    [HttpGet("location")]
    public object GetLocation(string address)
    {
       return AMap.GetLocation(address);
    }
}

```