using System;

namespace Shashlik.EfCore;

/// <summary>
/// 属性以json格式存储
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class JsonFieldAttribute : Attribute
{
}