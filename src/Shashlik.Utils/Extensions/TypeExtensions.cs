using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using JsonSerializer = RestSharp.Serialization.Json.JsonSerializer;

// ReSharper disable ConvertIfStatementToReturnStatement

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
        public static bool IsSubTypeOfGenericType(this Type childType, Type genericType)
        {
            var interfaceTypes = childType.GetTypeInfo().ImplementedInterfaces;

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (childType.IsGenericType && childType.GetGenericTypeDefinition() == genericType)
                return true;

            var baseType = childType.BaseType;
            if (baseType == null) return false;

            return IsSubTypeOfGenericType(baseType, genericType);
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ParseTo<T>(this object value)
        {
            return (T) ParseTo(value, typeof(T));
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="value"></param>
        /// <param name="destinationType">目标类型</param>
        /// <returns></returns>
        public static object ParseTo(this object value, Type destinationType)
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
                return Enum.ToObject(destinationType, (int) value);
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
                result = value.ParseTo(destinationType);
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
                result = value.ParseTo<T>();
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
            else if (objType.IsSubTypeOf<IDictionary>() || objType.IsSubTypeOfGenericType(typeof(IDictionary<,>)))
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

                props.ForEachItem(r =>
                {
                    if (!r.GetIndexParameters().IsNullOrEmpty()) return;

                    var value = r.GetValue(obj);
                    if (value == null || r.PropertyType.IsSimpleType())
                        dic[r.Name] = value;
                    else if (value is IDictionary || r.PropertyType.IsSubTypeOfGenericType(typeof(IDictionary<,>)))
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
                var value = GetObjectValue(obj, prop);
                return value;
            }
            else
            {
                var childPro = prop.TrimStart(propArr[0].ToCharArray()).TrimStart('.');
                var value = GetObjectValue(obj, propArr[0]);
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
        public static bool IsSubTypeOf(this Type type, Type parentType)
        {
            return parentType.IsAssignableFrom(type);
        }

        /// <summary>
        /// 是否为<typeparamref name="T"/>的子类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSubTypeOf<T>(this Type type)
        {
            return type.IsSubTypeOf(typeof(T));
        }

        /// <summary>
        /// 对象克隆
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T Clone<T>(this T obj)
        {
            if (obj != null)
            {
                return System.Text.Json.JsonSerializer.Deserialize<T>(System.Text.Json.JsonSerializer.Serialize(obj));
            }

            return default;
        }

        /// <summary>
        /// 是否定义了特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="member"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static bool IsDefinedAttribute<T>(this MemberInfo member, bool inherit)
            where T : Attribute
        {
            return Attribute.IsDefined(member, typeof(T), inherit);
        }

        /// <summary>
        /// 是否定义了特性
        /// </summary>
        /// <param name="member"></param>
        /// <param name="type"></param>
        /// <param name="inherit"></param>
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
            return type.IsSubTypeOf<IEnumerable>();
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

        /// <summary>
        /// 获取所有的父级类型,不包含object
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static HashSet<Type> GetAllBaseTypes(this Type type)
        {
            HashSet<Type> types = new HashSet<Type>();
            FillBaseType(types, type);
            return types;
        }

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
            return type.GetInterfaces().Except(type.BaseType.GetInterfaces());
        }

        /// <summary>
        /// json序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="options">序列化设置</param>
        /// <returns></returns>
        public static string ToJson<T>(this T obj, JsonSerializerOptions options = null)
            where T : class
        {
            if (obj == null)
                return null;
            return System.Text.Json.JsonSerializer.Serialize(obj, options);
        }

        /// <summary>
        /// json序列化,小驼峰命名
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJsonWithCamelCasePropertyNames<T>(this T obj)
            where T : class
        {
            if (obj == null)
                return null;
            return System.Text.Json.JsonSerializer.Serialize(obj, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });
        }

        /// <summary>
        /// 属性值浅拷贝,常用于Action&lt;Options&gt;,属性值拷贝
        /// </summary>
        /// <param name="source">源对象</param>
        /// <param name="dest">目标对象</param>
        /// <typeparam name="T"></typeparam>
        public static void CopyTo<T>(this T source, T dest)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (dest == null) throw new ArgumentNullException(nameof(dest));

            var type = typeof(T);

            foreach (var propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public |
                                                            BindingFlags.GetProperty))
            {
                if (propertyInfo.GetIndexParameters().Any())
                    return;
                var value = propertyInfo.GetValue(source);
                if (value == null)
                    return;
                propertyInfo.SetValue(dest, value);
            }
        }

        /// <summary>
        /// 获取json对象属性值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static T GetValue<T>(this JsonElement obj, string propertyName)
        {
            if (obj.ValueKind != JsonValueKind.Object)
                throw new ArgumentException($"Json value kind must be JsonValueKind.Object.");
            if (obj.TryGetProperty(propertyName, out var value))
            {
                switch (value.ValueKind)
                {
                    case JsonValueKind.Null:
                    case JsonValueKind.Undefined:
                        return default;
                }

                return (T) GetValue(value, typeof(T));
            }

            throw new ArgumentException($"Can not find json property \"{propertyName}\" in json {obj}");
        }

        public static object GetValue(this JsonElement obj, Type type, string propertyName)
        {
            if (obj.ValueKind != JsonValueKind.Object)
                throw new ArgumentException($"Json value kind must be JsonValueKind.Object.");
            if (obj.TryGetProperty(propertyName, out var value))
                return GetValue(value, type);

            throw new ArgumentException($"Can not find json property \"{propertyName}\" in json {obj}");
        }


        public static T GetValue<T>(this JsonElement obj)
        {
            switch (obj.ValueKind)
            {
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    return default;
            }

            return (T) GetValue(obj, typeof(T));
        }

        public static object GetValue(this JsonElement obj, Type type)
        {
            if (type == typeof(object))
                return obj;

            switch (obj.ValueKind)
            {
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    return default;
                case JsonValueKind.True:
                case JsonValueKind.False:
                {
                    if (type == typeof(bool) || type == typeof(bool?))
                        return obj.GetBoolean();

                    throw GetInvalidCastException(type, obj);
                }
                case JsonValueKind.Number:
                {
                    #region ->Int32

                    if (type == typeof(int) || type == typeof(int?))
                    {
                        if (obj.TryGetInt32(out var value))
                            return value;
                        else
                            throw GetInvalidCastException(type, obj);
                    }

                    #endregion

                    #region ->Int64

                    if (type == typeof(long) || type == typeof(long?))
                    {
                        if (obj.TryGetInt64(out var value))
                            return value;
                        else
                            throw GetInvalidCastException(type, obj);
                    }

                    #endregion

                    #region ->Enum

                    {
                        Type enumType = null;
                        if (type.IsEnum)
                            enumType = type;
                        else if (type.IsSubTypeOfGenericType(typeof(Nullable<>))
                                 && type.GetGenericArguments().First().IsEnum)
                            enumType = type.GetGenericArguments().First();

                        if (enumType != null)
                        {
                            if (obj.TryGetInt32(out var intValue))
                            {
                                if (Enum.TryParse(enumType, intValue.ToString(), true, out var result))
                                    return result;
                            }
                            else
                                throw GetInvalidCastException(type, obj);
                        }
                    }

                    #endregion

                    #region ->float

                    if (type == typeof(float) || type == typeof(float?))
                    {
                        if (obj.TryGetSingle(out var value))
                            return value;
                        else
                            throw GetInvalidCastException(type, obj);
                    }

                    #endregion

                    #region ->double

                    if (type == typeof(double) || type == typeof(double?))
                    {
                        if (obj.TryGetDouble(out var value))
                            return value;
                        else
                            throw GetInvalidCastException(type, obj);
                    }

                    #endregion

                    #region ->decimal

                    if (type == typeof(decimal) || type == typeof(decimal?))
                    {
                        if (obj.TryGetDecimal(out var value))
                            return value;
                        else
                            throw GetInvalidCastException(type, obj);
                    }

                    #endregion

                    #region ->Int16

                    if (type == typeof(short) || type == typeof(short?))
                    {
                        if (obj.TryGetInt16(out var value))
                            return value;
                        else
                            throw GetInvalidCastException(type, obj);
                    }

                    #endregion

                    #region ->UInt32

                    if (type == typeof(uint) || type == typeof(uint?))
                    {
                        if (obj.TryGetUInt32(out var value))
                            return value;
                        else
                            throw GetInvalidCastException(type, obj);
                    }

                    #endregion

                    #region ->UInt64

                    if (type == typeof(ulong) || type == typeof(ulong?))
                    {
                        if (obj.TryGetUInt64(out var value))
                            return value;
                        else
                            throw GetInvalidCastException(type, obj);
                    }

                    #endregion

                    #region ->UInt16

                    if (type == typeof(ushort) || type == typeof(ushort?))
                    {
                        if (obj.TryGetUInt16(out var value))
                            return value;
                        else
                            throw GetInvalidCastException(type, obj);
                    }

                    #endregion

                    #region ->byte

                    if (type == typeof(byte) || type == typeof(byte?))
                    {
                        if (obj.TryGetByte(out var value))
                            return value;
                        else
                            throw GetInvalidCastException(type, obj);
                    }

                    #endregion

                    #region ->sbyte

                    if (type == typeof(sbyte) || type == typeof(sbyte?))
                    {
                        if (obj.TryGetSByte(out var value))
                            return value;
                        else
                            throw GetInvalidCastException(type, obj);
                    }

                    #endregion

                    throw GetInvalidCastException(type, obj);
                }
                case JsonValueKind.String:
                {
                    #region ->DateTime

                    if (type == typeof(DateTime) || type == typeof(DateTime?))
                    {
                        if (obj.TryGetDateTime(out var datetime))
                            return datetime;
                        throw GetInvalidCastException(type, obj);
                    }

                    #endregion

                    #region ->DateTimeOffset

                    if (type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?)
                    )
                    {
                        if (obj.TryGetDateTimeOffset(out var datetime))
                            return datetime;
                        throw GetInvalidCastException(type, obj);
                    }

                    #endregion

                    #region ->Enum

                    Type enumType = null;
                    if (type.IsEnum)
                    {
                        enumType = type;
                    }
                    else if (type.IsSubTypeOfGenericType(typeof(Nullable<>))
                             && type.GetGenericArguments().First().IsEnum)
                    {
                        enumType = type.GetGenericArguments().First();
                    }

                    if (enumType != null)
                    {
                        var str = obj.GetString();
                        if (str.IsNullOrWhiteSpace())
                            throw GetInvalidCastException(type, obj);

                        if (Enum.TryParse(enumType, obj.GetString(), true, out var result))
                            return result;

                        throw GetInvalidCastException(type, obj);
                    }

                    #endregion

                    #region ->Guid

                    if (type == typeof(string))
                        return obj.GetString();

                    if (type == typeof(Guid) || type == typeof(Guid?))
                    {
                        if (obj.TryGetGuid(out var guid))
                            return guid;
                        throw GetInvalidCastException(type, obj);
                    }

                    #endregion

                    throw GetInvalidCastException(type, obj);
                }
                case JsonValueKind.Object:
                case JsonValueKind.Array:
                {
                    var bufferWriter = new ArrayBufferWriter<byte>();
                    using (var writer = new Utf8JsonWriter(bufferWriter))
                    {
                        obj.WriteTo(writer);
                    }

                    return System.Text.Json.JsonSerializer.Deserialize(bufferWriter.WrittenSpan, type, null);
                }
                default:
                    throw GetInvalidCastException(type, obj);
            }
        }

        #region private

        private static object JToken2Object(JToken token)
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

        private static object JTokenValue(JToken token)
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

        private static object JsonElement2Object(JsonElement obj)
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

        private static object JsonElementValue(JsonElement obj)
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

        private static (bool exists, object value) GetObjectValue(object obj, string proName)
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
            else if (objType.IsSubTypeOfGenericType(typeof(IDictionary<,>)))
            {
                var list = (obj as IEnumerable)!.OfType<dynamic>();
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
                var pro = objType.GetProperty(proName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
                if (pro == null) return (false, null);
                return (true, pro.GetValue(obj));
            }
        }

        private static void FillBaseType(HashSet<Type> results, Type type)
        {
            if (type.BaseType != typeof(object))
            {
                results.Add(type.BaseType);
                FillBaseType(results, type.BaseType);
            }
        }

        private static InvalidCastException GetInvalidCastException(Type type, JsonElement json)
        {
            return new InvalidCastException($"Can not convert to type:{type} from json value: {json}");
        }

        #endregion
    }
}