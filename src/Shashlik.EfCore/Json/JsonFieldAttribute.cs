using System;

namespace Shashlik.EfCore.Json;

/// <summary>
/// 属性以json格式存储(Newtonsoft.Json)
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class JsonFieldAttribute : Attribute
{
}