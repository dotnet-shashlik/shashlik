using Shashlik.Utils.Common;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Xml.Serialization;

namespace Shashlik.Utils.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// 判断给定的类型是否继承自<paramref name="genericType"/>泛型类型,
        /// <para>
        /// 例typeof(Child).IsChildTypeOfGenericType(typeof(IParent&lt;&gt;));
        /// </para>
        /// </summary>
        /// <param name="childType">子类型</param>
        /// <param name="genericType">泛型父级,例:typeof(IParent&lt;&gt;)</param>
        /// <returns></returns>
        public static bool IsChildTypeOfGenericType(this Type childType, Type genericType)
        {
            var interfaceTypes = childType.GetTypeInfo().ImplementedInterfaces;

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (childType.IsGenericType && childType.GetGenericTypeDefinition() == genericType)
                return true;

            Type baseType = childType.BaseType;
            if (baseType == null) return false;

            return IsChildTypeOfGenericType(baseType, genericType);
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ConvertTo<T>(this object value)
        {
            if (value == null)
                return default(T);

            if (value is T)
                return (T)value;

            var destinationType = typeof(T);
            var sourceType = value.GetType();
            if (destinationType == typeof(bool) || destinationType == typeof(bool?))
                value = Convert.ToBoolean(value);

            TypeConverter destinationConverter = TypeDescriptor.GetConverter(destinationType);
            TypeConverter sourceConverter = TypeDescriptor.GetConverter(sourceType);
            if (destinationConverter != null && destinationConverter.CanConvertFrom(value.GetType()))
                return (T)destinationConverter.ConvertFrom(value);
            if (sourceConverter != null && sourceConverter.CanConvertTo(destinationType))
                return (T)sourceConverter.ConvertTo(value, destinationType);
            if (destinationType.IsEnum && value is int)
                return (T)Enum.ToObject(destinationType, (int)value);
            if (!destinationType.IsInstanceOfType(value))
                return (T)Convert.ChangeType(value, destinationType);
            return (T)value;
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="value"></param>
        /// <param name="destinationType">目标类型</param>
        /// <returns></returns>
        public static object ConvertTo(this object value, Type destinationType)
        {
            if (value == null)
                return null;

            var sourceType = value.GetType();
            if (destinationType == typeof(bool) || destinationType == typeof(bool?))
                return Convert.ToBoolean(value);

            TypeConverter destinationConverter = TypeDescriptor.GetConverter(destinationType);
            TypeConverter sourceConverter = TypeDescriptor.GetConverter(sourceType);
            if (destinationConverter != null && destinationConverter.CanConvertFrom(value.GetType()))
                return destinationConverter.ConvertFrom(value);
            if (sourceConverter != null && sourceConverter.CanConvertTo(destinationType))
                return sourceConverter.ConvertTo(value, destinationType);
            if (destinationType.IsEnum && value is int)
                return Enum.ToObject(destinationType, (int)value);
            if (!destinationType.IsInstanceOfType(value))
                return Convert.ChangeType(value, destinationType);

            throw new Exception($"[{value.GetType()}:{value}]转换为目标类型:[{destinationType}]无效!");
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(this object value, Type destinationType, out object result)
        {
            try
            {
                result = value.ConvertTo(destinationType);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryParse<T>(this object value, out T result) where T : struct
        {
            try
            {
                result = value.ConvertTo<T>();
                return true;
            }
            catch
            {
                result = default(T);
                return false;
            }
        }

        /// <summary>
        /// 将对象属性转换为字典,复杂类型会递归转换,支持JsonElement/IDictionary/JsonObject等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IDictionary<string, object> MapToDictionary<TModel>(this TModel obj)
        {
            if (obj == null)
                return null;
            var dic = new Dictionary<string, object>();
            var objType = obj.GetType();
            if (obj is JToken jToken)
            {
                if (jToken.Type != JTokenType.Object)
                    return null;
                return JToken2Object(jToken) as IDictionary<string, object>;
            }
            else if (objType.IsChildTypeOf<IDictionary>() || objType.IsChildTypeOfGenericType(typeof(IDictionary<,>)))
            {
                return (obj as IEnumerable)
                    .OfType<dynamic>()
                    .ToDictionary<dynamic, string, object>(
                    r => r.Key.ToString(),
                    r => IsSimpleType(r.Value.GetType()) ? r.Value : MapToDictionary(r.Value));

            }
            else if (obj is JsonElement json)
            {
                if (json.ValueKind != JsonValueKind.Object)
                    return null;

                return JsonElement2Object(json) as IDictionary<string, object>;
            }
            else
            {
                var props = objType
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)
                    .Where(r => !r.GetIndexParameters().Any());

                props.Foreach(r =>
                {
                    if (!r.GetIndexParameters().IsNullOrEmpty()) return;

                    var value = r.GetValue(obj);
                    if (value == null || r.PropertyType.IsSimpleType())
                        dic[r.Name] = value;
                    else if (value is IDictionary || r.PropertyType.IsChildTypeOfGenericType(typeof(IDictionary<,>)))
                        dic[r.Name] = MapToDictionary(value);
                    else if (value is IEnumerable list)
                    {
                        dic[r.Name] = list.OfType<object>().Select(r =>
                        {
                            if (r == null || r.GetType().IsSimpleType()) return r;
                            else return MapToDictionary(r);
                        }).ToList();
                    }
                    else dic[r.Name] = MapToDictionary(value);
                });

                return dic;
            }
        }

        static object JToken2Object(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    {
                        var dic = new Dictionary<string, object>();
                        foreach (var item in token.Value<JObject>())
                        {
                            dic[item.Key] = JToken2Object(item.Value);
                        }
                        return dic;
                    }

                case JTokenType.Array:
                    {
                        List<object> children = new List<object>();
                        foreach (var item in token.Value<JArray>())
                            children.Add(JToken2Object(item));
                        return children;
                    }

                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.String:
                case JTokenType.Boolean:
                case JTokenType.Bytes:
                case JTokenType.Raw:
                case JTokenType.Null:
                    return token.ToObject<object>();
                default:
                    throw new FormatException("json format error!");
            }
        }

        static object JTokenValue(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    return token.Value<JObject>();
                case JTokenType.Array:
                    return token.Value<JArray>();

                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.String:
                case JTokenType.Boolean:
                case JTokenType.Bytes:
                case JTokenType.Raw:
                case JTokenType.Null:
                case JTokenType.Date:
                case JTokenType.Uri:
                case JTokenType.TimeSpan:
                case JTokenType.Guid:
                case JTokenType.Undefined:
                case JTokenType.None:
                    return token.ToObject<object>();
                default:
                    throw new FormatException("json format error!");
            }
        }

        static object JsonElement2Object(JsonElement obj)
        {
            switch (obj.ValueKind)
            {
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    return null;
                case JsonValueKind.String:
                    {
                        if (obj.TryGetDateTime(out var datetime))
                            return datetime;
                        else if (obj.TryGetDateTimeOffset(out var datetimeOffset))
                            return datetimeOffset;
                        return obj.GetString();
                    }
                case JsonValueKind.True:
                case JsonValueKind.False:
                    return obj.GetBoolean();
                case JsonValueKind.Number:
                    {
                        if (obj.TryGetInt64(out var value2))
                        {
                            return value2;
                        }
                        else if (obj.TryGetDecimal(out var value3))
                        {
                            return value3;
                        }
                        return null;
                    }
                case JsonValueKind.Object:
                    {
                        var dic = new Dictionary<string, object>();
                        foreach (var item in obj.EnumerateObject())
                        {
                            dic[item.Name] = JsonElement2Object(item.Value);
                        }
                        return dic;
                    }
                case JsonValueKind.Array:
                    {
                        List<object> children = new List<object>();
                        foreach (var item in obj.EnumerateArray())
                            children.Add(JsonElement2Object(item));
                        return children;
                    }
                default:
                    throw new FormatException("json format error!");
            }
        }

        static object JsonElementValue(JsonElement obj)
        {
            switch (obj.ValueKind)
            {
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    return null;
                case JsonValueKind.String:
                    {
                        if (obj.TryGetDateTime(out var datetime))
                            return datetime;
                        else if (obj.TryGetDateTimeOffset(out var datetimeOffset))
                            return datetimeOffset;
                        return obj.GetString();
                    }
                case JsonValueKind.True:
                case JsonValueKind.False:
                    return obj.GetBoolean();
                case JsonValueKind.Number:
                    {
                        if (obj.TryGetInt64(out var value2))
                        {
                            return value2;
                        }
                        else if (obj.TryGetDecimal(out var value3))
                        {
                            return value3;
                        }
                        return null;
                    }
                case JsonValueKind.Object:
                case JsonValueKind.Array:
                    return obj;
                default:
                    throw new FormatException("json format error!");
            }
        }

        static (bool exists, object value) getObjectValue(object obj, string proName)
        {
            var objType = obj.GetType();
            if (obj is JToken jToken)
            {
                JToken json = null;
                if (jToken.Type == JTokenType.Array && int.TryParse(proName, out var index))
                {
                    var jsonArr = jToken.Value<JArray>();
                    if (jsonArr.Count() > index)
                        return (false, null);
                    json = jsonArr.ElementAt(index);
                }
                else if (jToken.Type == JTokenType.Object)
                {
                    try
                    {
                        json = jToken[proName];
                    }
                    catch (KeyNotFoundException)
                    {
                        return (false, null);
                    }
                }
                else
                    return (false, null);

                if (json != null)
                    return (true, JTokenValue(json));
                return (false, null);
            }
            else if (objType.IsChildTypeOfGenericType(typeof(IDictionary<,>)))
            {
                var list = (obj as IEnumerable).OfType<dynamic>();
                try
                {
                    var el = list.First(r => r.Key?.ToString() == proName);
                    return (true, el.Value);
                }
                catch (InvalidOperationException)
                {
                    return (false, null);
                }
            }
            else if (obj is IDictionary dic)
            {
                if (dic.Contains(proName))
                    return (true, dic[proName]);
                return (false, null);
            }
            else if (obj is JsonElement jsonObj)
            {
                JsonElement? json = null;
                if (jsonObj.ValueKind == JsonValueKind.Array && int.TryParse(proName, out var index))
                {
                    var jsonArr = jsonObj.EnumerateArray();
                    if (jsonArr.Count() > index)
                        return (false, null);
                    json = jsonObj.EnumerateArray().ElementAt(index);
                }
                else if (jsonObj.ValueKind == JsonValueKind.Object)
                {
                    try
                    {
                        json = jsonObj.GetProperty(proName);
                    }
                    catch (KeyNotFoundException)
                    {
                        return (false, null);
                    }
                }
                else
                    return (false, null);

                if (json.HasValue)
                    return (true, JsonElementValue(json.Value));
                return (false, null);
            }
            else if (objType is IEnumerable list && int.TryParse(proName, out var index))
            {
                var i = 0;
                foreach (var item in list)
                {
                    if (i == index)
                        return (true, item);
                    i++;
                }
                return (true, null);
            }
            else
            {
                var pro = objType.GetProperty(proName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
                if (pro == null) return (false, null);
                return (true, pro.GetValue(obj));
            }
        }

        /// <summary>
        /// 获取属性值，支持JsonElement/IDictionary/JsonObject
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="obj"></param>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static (bool exists, object value) GetPropertyValue<TModel>(this TModel obj, string prop)
        {
            if (string.IsNullOrWhiteSpace(prop))
                throw new ArgumentException($"“{nameof(prop)}”不能为 Null 或空白", nameof(prop));

            var propArr = prop.Split('.');

            if (propArr.Length == 1)
            {
                var value = getObjectValue(obj, prop);
                return value;
            }
            else
            {
                var childPro = prop.TrimStart(propArr[0].ToCharArray()).TrimStart('.');
                var value = getObjectValue(obj, propArr[0]);
                if (!value.exists)
                    return (false, null);
                return GetPropertyValue(value.value, childPro);
            }
        }

        /// <summary>
        /// 是否为<paramref name="parentType"/>的子类
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parentType"></param>
        /// <returns></returns>
        public static bool IsChildTypeOf(this Type type, Type parentType)
        {
            return parentType.IsAssignableFrom(type);
        }

        /// <summary>
        /// 是否为<typeparamref name="T"/>的子类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsChildTypeOf<T>(this Type type)
        {
            return type.IsChildTypeOf(typeof(T));
        }

        /// <summary>
        /// 对象克隆
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T Clone<T>(this T obj)
        {
            T ret = default(T);
            if (obj != null)
            {
                ret = JsonConvert.DeserializeObject<T>(JsonHelper.Serialize(obj));
            }
            return ret;
        }

        /// <summary>
        /// 是否定义了特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsDefinedAttribute<T>(this MemberInfo member, bool inherit)
            where T : Attribute
        {
            return Attribute.IsDefined(member, typeof(T), inherit);
        }

        /// <summary>
        /// 是否定义了特性
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsDefinedAttribute(this MemberInfo member, Type type, bool inherit)
        {
            return Attribute.IsDefined(member, type, inherit);
        }

        /// <summary>
        /// 类型是否为简单类型(自定义的结构体非简单类型)
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsSimpleType(this Type type)
        {
            /*
             * copy from System.Data.Linq.SqlClient.TypeSystem
             * **/

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                type = type.GetGenericArguments()[0];

            if (type.IsEnum)
                return true;

            if (type == typeof(Guid))
                return true;

            TypeCode tc = Type.GetTypeCode(type);
            switch (tc)
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.Char:
                case TypeCode.String:
                case TypeCode.Boolean:
                case TypeCode.DateTime:
                    return true;
                case TypeCode.Object:
                    return (typeof(TimeSpan) == type) || (typeof(DateTimeOffset) == type);
                default:
                    return false;
            }
        }

        /// <summary>
        /// 类型是否为集合类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsCollectionType(this Type type)
        {
            return type.IsChildTypeOf<IEnumerable>();

        }

        /// <summary>
        /// 类型是否为委托类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsDelegate(this Type type)
        {
            return typeof(Delegate).IsAssignableFrom(type);
        }

        private static void fillBaseType(HashSet<Type> results, Type type)
        {
            if (type.BaseType != typeof(object))
            {
                results.Add(type.BaseType);
                fillBaseType(results, type.BaseType);
            }
        }

        /// <summary>
        /// 获取所有的父级类型,不包含接口和object
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static HashSet<Type> GetAllBaseTypes(this Type type)
        {
            HashSet<Type> types = new HashSet<Type>();
            fillBaseType(types, type);
            return types;
        }

        #region 配置转换 jObject

        /// <summary>
        /// 将配置对象转换为jobject格式,但是转换出来的数据只能是string类型
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static JObject GetJObject(this IConfigurationSection configuration)
        {
            JObject obj = new JObject();
            jObject(obj, configuration);
            return obj;
        }

        private static void jObject(JObject obj, IConfigurationSection configurationSection)
        {
            var children = configurationSection.GetChildren();

            foreach (var child in children)
            {
                var child_child = child.GetChildren();
                if (child_child.IsNullOrEmpty())
                {
                    // 值
                    obj.Add(child.Key, new JValue(configurationSection.GetValue<string>(child.Key)));
                }
                else if (child_child.FirstOrDefault()?.Key == "0")
                {
                    // 数组
                    var array = new JArray();
                    jArray(array, child);
                    obj.Add(child.Key, array);
                }

                else
                {
                    var childrenObj = new JObject();
                    // 对象
                    jObject(childrenObj, child);
                    obj.Add(child.Key, childrenObj);
                }
            }
        }
        private static void jArray(JArray jArray, IConfigurationSection configurationSection)
        {
            foreach (var item in configurationSection.GetChildren())
            {
                var children = item.GetChildren();

                if (children.IsNullOrEmpty())
                {
                    // 值
                    jArray.Add(new JValue(item.Value));
                }
                else if (children.OrderBy(r => r.Key).First().Key == "0")
                {
                    var childrenArray = new JArray();
                    TypeExtensions.jArray(childrenArray, item);
                    jArray.Add(childrenArray);
                }

                else
                {
                    var childrenObj = new JObject();
                    // 对象
                    jObject(childrenObj, item);
                    jArray.Add(childrenObj);
                }
            }
        }

        #endregion

        /// <summary>
        /// 获取实现的接口
        /// </summary>
        /// <param name="type"></param>
        /// <param name="includeInherited">是否包含继承父类的接口</param>
        /// <returns></returns>
        public static IEnumerable<Type> GetInterfaces(this Type type, bool includeInherited)
        {
            if (includeInherited || type.BaseType == null)
                return type.GetInterfaces();
            else
                return type.GetInterfaces().Except(type.BaseType.GetInterfaces());
        }

        /// <summary>
        /// json序列化,默认设置ReferenceLoopHandling.Ignore,Formatting.Indented
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="jsonSerializerSettings"></param>
        /// <returns></returns>
        public static string ToJson<T>(this T obj, JsonSerializerSettings jsonSerializerSettings = null)
            where T : class
        {
            if (obj == null)
                return null;
            return JsonHelper.Serialize(obj, jsonSerializerSettings);
        }

        /// <summary>
        /// json序列化,默认设置ReferenceLoopHandling.Ignore,Formatting.Indented
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="jsonSerializerSettings"></param>
        /// <returns></returns>
        public static string ToJsonWithCamelCasePropertyNames<T>(this T obj)
            where T : class
        {
            if (obj == null)
                return null;
            return JsonConvert.SerializeObject(obj,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.Indented,
                    // 默认小驼峰用法
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                });
        }

        /// <summary>
        /// 时间戳转换位DateTime,本地时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this long time)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToLocalTime().AddSeconds(time);
        }
    }
}
