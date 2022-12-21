using System;

namespace Shashlik.EfCore.Json;

/// <summary>
/// 属性以json 字符串格式存储(Newtonsoft.Json)<para></para>
/// ef core7已增加json字段特性,功能更强,ef core7及以上版本建议使用新特性
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class JsonFieldAttribute : Attribute
{
}