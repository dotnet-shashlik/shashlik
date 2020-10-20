using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using Shashlik.Utils.Extensions;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable PublicConstructorInAbstractClass
// ReSharper disable UseDeconstruction
// ReSharper disable MemberCanBeProtected.Global

namespace Shashlik.JsonPatch
{
    /// <summary>
    /// 按需 部分更新,错误的属性将被忽略
    /// </summary>
    public abstract class PatchUpdateBase
    {
        /// <summary>
        /// 原始数据
        /// </summary>
        public JsonElement Origin { get; }

        /// <summary>
        /// 属性转换器
        /// </summary>
        private IDictionary<string, PatchUpdateConverter> Converters { get; set; } =
            new Dictionary<string, PatchUpdateConverter>();

        /// <summary>
        /// 排除更新
        /// </summary>
        private List<string> SourceExcludes { get; set; } = new List<string>();

        /// <summary>
        /// 有效值
        /// </summary>
        public Dictionary<string, object> Values { get; } =
            new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

        public PatchUpdateBase(JsonElement jobject)
        {
            Origin = jobject;
            var type = GetType();

            var enumerateObject = Origin.EnumerateObject();
            while (enumerateObject.MoveNext())
            {
                var name = enumerateObject.Current.Name;
                var jsonElement = enumerateObject.Current.Value;

                var sourcePropertyInfo = this.GetType().GetProperty(name,
                    BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.SetProperty);
                if (sourcePropertyInfo == null || !sourcePropertyInfo.GetIndexParameters().IsNullOrEmpty())
                    throw new ArgumentException($"Can not find property {name} in {type}.");

                var sourceType = sourcePropertyInfo.PropertyType;
                var sourceValue = jsonElement.GetValue(sourceType);
                sourcePropertyInfo.SetValue(this, sourceValue);
                Values.Add(name, sourceValue);
            }
        }

        public virtual void UpdateTo<TEntity>(TEntity entity)
            where TEntity : class
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var type = entity.GetType();

            foreach (var item in Values)
            {
                if (SourceExcludes.Any(r => r.EqualsIgnoreCase(item.Key)))
                    continue;

                var targetPro = item.Key;
                var targetValue = item.Value;
                if (Converters.TryGetValue(item.Key, out var converter))
                {
                    targetPro = converter.TargetPro;
                    targetValue = converter.ConvertFunction(item.Value);
                }

                var pro = type.GetProperty(targetPro,
                    BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.SetProperty);
                if (pro == null || !pro.GetIndexParameters().IsNullOrEmpty())
                    continue;

                pro.SetValue(entity, targetValue);
            }
        }

        /// <summary>
        /// 排除源字段,不允许更新,一般用于不同的权限用户更新数据用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        public void Exclude<T>(params Expression<Func<T, object>>[] expression)
        {
            expression.ForEachItem(r => SourceExcludes.Add(r.GetPropertyName()));
        }

        /// <summary>
        /// 排除源字段,不允许更新,一般用于不同的权限用户更新数据用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Exclude<T>(params string[] excludes)
        {
            this.SourceExcludes.AddRange(excludes);
        }

        /// <summary>
        /// 增加自定义转换器
        /// </summary>
        /// <param name="sourceProName">源字段名</param>
        /// <param name="targetProName">目标类型字段名称</param>
        /// <param name="convert">转换方法</param>
        public void AddConvert(string sourceProName, string targetProName, Func<object, object> convert)
        {
            Converters.Add(sourceProName, new PatchUpdateConverter(sourceProName, targetProName, convert));
        }
    }
}