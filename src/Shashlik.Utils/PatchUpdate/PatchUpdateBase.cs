using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Shashlik.Utils.Extensions;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable PublicConstructorInAbstractClass
// ReSharper disable UseDeconstruction
// ReSharper disable MemberCanBeProtected.Global

namespace Shashlik.Utils.PatchUpdate
{
    /// <summary>
    /// 按需 部分更新,错误的属性将被忽略
    /// </summary>
    public abstract class PatchUpdateBase
    {
        /// <summary>
        /// 原始数据
        /// </summary>
        public JObject Origin { get; }

        private List<string> excludes { get; set; } = new List<string>();

        /// <summary>
        /// 有效值
        /// </summary>
        protected Dictionary<string, object> ValidPros { get; }

        public PatchUpdateBase(JObject jObject)
        {
            Origin = jObject;
            var type = GetType();
            ValidPros = new Dictionary<string, object>();
            foreach (var item in jObject)
            {
                var pro = type.GetProperty(item.Key,
                    BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.SetProperty);
                if (pro == null || !pro.GetIndexParameters().IsNullOrEmpty())
                    continue;
                try
                {
                    var value = (item.Value.Type == JTokenType.Null) ? null : item.Value.ToObject(pro.PropertyType);
                    ValidPros.Add(pro.Name, value);
                    pro.SetValue(this, value);
                }
                catch
                {
                    continue;
                }
            }
        }

        public virtual void UpdateTo<TEntity>(TEntity entity)
            where TEntity : class
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var type = entity.GetType();

            IList<PatchUpdateConverter> converters = new List<PatchUpdateConverter>();
            Converter(converters);

            foreach (var item in ValidPros)
            {
                if (excludes.Any(r => r.EqualsIgnoreCase(item.Key)))
                    continue;
                var targetPro = item.Key;
                var targetValue = item.Value;
                var converter = converters.LastOrDefault(r => r.SourcePro.EqualsIgnoreCase(item.Key));
                if (converter != null)
                {
                    var res = converter.Convert(item.Value);
                    targetPro = res.targetPro;
                    targetValue = res.targetValue;
                }

                var pro = type.GetProperty(targetPro,
                    BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.SetProperty);
                if (pro == null || !pro.GetIndexParameters().IsNullOrEmpty())
                    continue;

                pro.SetValue(entity, targetValue);
            }
        }

        /// <summary>
        /// 自定义属性转换器,string:源字段(input的字段名称), object:源字段值, (string:目标字段(entity), , object:目标字段值)
        /// </summary>
        /// <param name="converters"></param>
        public virtual void Converter(IList<PatchUpdateConverter> converters)
        {
        }

        /// <summary>
        /// 排除字段,不允许更新,一般用于不同的权限用户更新数据用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        public void Exclude<T>(params Expression<Func<T, object>>[] expression)
        {
            expression.ForEachItem(r => excludes.Add(r.GetPropertyName()));
        }

        /// <summary>
        /// 排除字段,不允许更新,一般用于不同的权限用户更新数据用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Exclude<T>(params string[] excludes)
        {
            this.excludes.AddRange(excludes);
        }
    }
}