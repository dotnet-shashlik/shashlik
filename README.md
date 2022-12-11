# Shashlik

[![build and test](https://github.com/dotnet-shashlik/shashlik/workflows/build%20and%20test/badge.svg)](https://github.com/dotnet-shashlik/shashlik)
[![license](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/dotnet-shashlik/shashlik/blob/main/LICENSE)

---

Shashlik，.NET快速集成基础框架，旨在通过配置文件、约定等方式做到简化各种代码配置、模块配置、服务配置，提高程序集的可复用性。现已集成`AutoMapper`、`Sms`、`Captcha`、`Redis`等常用框架和功能，只需要导入相应的程序集，添加对应的配置文件，几乎无需做任何代码改动，做到开箱即用。

Shashlik.Kernel是整个Shashlik的核心程序集，几乎所有的Shashlik组件都基于Shashlik.Kernel进行构建。其包含了约定式服务注册、配置装配、自动装配等核心功能。

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

是的，就这样完了，一个可复用的地理信息逆编码功能库就完成了，简化了大量的配置处理、服务注册代码。

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