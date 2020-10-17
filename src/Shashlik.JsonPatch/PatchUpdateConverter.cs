using System;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Shashlik.JsonPatch
{
    /// <summary>
    /// 自定义属性值转换器
    /// </summary>
    public class PatchUpdateConverter
    {
        public PatchUpdateConverter(string sourcePro, string targetPro, Func<object, object> convertFunction)
        {
            SourcePro = sourcePro;
            TargetPro = targetPro;
            ConvertFunction = convertFunction;
        }

        public string SourcePro { get; }

        public string TargetPro { get; }

        public Func<object, object> ConvertFunction { get; }
    }
}