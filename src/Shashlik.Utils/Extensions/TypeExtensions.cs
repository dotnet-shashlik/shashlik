using Newtonsoft.Json.Linq;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.Json;

// ReSharper disable ConditionIsAlwaysTrueOrFalse

namespace Shashlik.Utils.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// 判断给定的类型是否继承自<paramref name="genericType"/>泛型类型,
        /// <para>
        /// e.g.: typeof(Child&lt;&gt;).IsSubTypeOfGenericType(typeof(IParent&lt;&gt;));  result->true 
        /// </para>
        /// <para>
        /// e.g.: typeof(Child&lt;int&gt;).IsSubTypeOfGenericType(typeof(IParent&lt;&gt;));  result->true 
        /// </para>
        /// </summary>
        /// <param name="childType">子类型</param>
        /// <param name="genericType">泛型父级,例: typeof(IParent&lt;&gt;)</param>
        /// <returns></returns>
        public static bool IsSubTypeOfGenericType(this Type childType, Type genericType)
        {
            if (childType == genericType)
                return false;
            if (!genericType.IsGenericTypeDefinition)
                return false;
            var interfaceTypes = childType.GetTypeInfo().ImplementedInterfaces;

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (childType.IsGenericType && childType.GetGenericTypeDefinition() == genericType)
                return true;

            var baseType = childType.BaseType;
            if (baseType is null) return false;

            return IsSubTypeOfGenericType(baseType, genericType);
        }

        /// <summary>
        /// 判断给定的类型是否继承自<paramref name="genericType"/>泛型类型,
        /// <para>
        /// e.g.: typeof(Child&lt;&gt;).IsSubTypeOfGenericType(typeof(IParent&lt;&gt;));  result->true 
        /// </para>
        /// <para>
        /// e.g.: typeof(Child&lt;int&gt;).IsSubTypeOfGenericType(typeof(IParent&lt;&gt;));  result->false 
        /// </para>
        /// </summary>
        /// <param name="childType">子类型</param>
        /// <param name="genericType">泛型父级,例: typeof(IParent&lt;&gt;)</param>
        /// <returns></returns>
        public static bool IsSubTypeOfGenericDefinitionType(this Type childType, Type genericType)
        {
            if (!genericType.IsGenericTypeDefinition)
                return false;
            if (!childType.IsGenericTypeDefinition)
                return false;
            return childType.GetAllBaseTypes().Any(r => r == genericType)
                   || childType.GetAllInterfaces(true).Any(r => r == genericType);
        }

        /// <summary>
        /// 是否为可空类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullableType(this Type type)
        {
            var typeInfo = type.GetTypeInfo();

            return !typeInfo.IsValueType
                   || (typeInfo.IsGenericType
                       && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        /// 获取指定的构造函数
        /// </summary>
        /// <param name="type"></param>
        /// <param name="types">构造函数的参数类型匹配</param>
        /// <returns></returns>
        public static ConstructorInfo? GetDeclaredConstructor(this Type type, params Type[] types)
        {
            if (types is null) throw new ArgumentNullException(nameof(types));
            return type.GetTypeInfo()
                .DeclaredConstructors
                .SingleOrDefault(r =>
                {
                    var ps = r.GetParameters();
                    if (ps.Length != types.Length)
                        return false;
                    for (int i = 0; i < ps.Length; i++)
                    {
                        if (ps[i].ParameterType != types[i])
                            return false;
                    }

                    return true;
                });
        }

        /// <summary>
        /// 获取类型默认值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object? GetDefaultValue(this Type type)
        {
            if (!type.GetTypeInfo().IsValueType)
                return null;
            return CommonTypeDictionary.TryGetValue(type, out var value)
                ? value
                : Activator.CreateInstance(type);
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T? ParseTo<T>(this object value)
        {
            var res = ParseTo(value, typeof(T));
            if (res is null) return default;
            return (T)res;
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="value"></param>
        /// <param name="destinationType">目标类型</param>
        /// <returns></returns>
        public static object? ParseTo(this object? value, Type destinationType)
        {
            if (value is null)
                return null;

            if (destinationType.IsInstanceOfType(value))
                return value;

            var sourceType = value.GetType();
            if (destinationType == typeof(bool) || destinationType == typeof(bool?))
                return Convert.ToBoolean(value);

            var destinationConverter = TypeDescriptor.GetConverter(destinationType);
            var sourceConverter = TypeDescriptor.GetConverter(sourceType);
            if (destinationConverter.CanConvertFrom(sourceType))
                return destinationConverter.ConvertFrom(value);
            if (sourceConverter.CanConvertTo(destinationType))
                return sourceConverter.ConvertTo(value, destinationType);
            if (destinationType.IsEnum)
            {
                var str = value.ToString();
                if (str is not null)
                    return Enum.Parse(destinationType, str);
            }

            throw new InvalidCastException($"Invalid cast to type {destinationType} from {sourceType}");
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="value"></param>
        /// <param name="destinationType">目标类型</param>
        /// <param name="result">转换结果</param>
        /// <returns></returns>
        public static bool TryParse(this object? value, Type destinationType, out object? result)
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
        /// <param name="result">转换后的值</param>
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
                result = default;
                return false;
            }
        }

        /// <summary>
        /// 将对象属性转换为字典,复杂类型会递归转换,支持JsonElement/IDictionary/JsonObject等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IDictionary<string, object?> MapToDictionary<TModel>(this TModel obj)
        {
            if (obj is null)
                throw new InvalidCastException();
            var dic = new Dictionary<string, object?>();
            var objType = obj.GetType();
            if (obj is JToken jToken)
            {
                if (jToken.Type != JTokenType.Object)
                    throw new InvalidCastException();
                return (JToken2Object(jToken) as IDictionary<string, object?>)!;
            }
            else if (obj is IDictionary || objType.IsSubTypeOfGenericType(typeof(IDictionary<,>)))
            {
                return ((IEnumerable)obj)
                        .OfType<dynamic>()
                        .ToDictionary<dynamic, string, object?>(
                            r => r.Key.ToString(),
                            r => IsSimpleType(r.Value.GetType()) ? r.Value : MapToDictionary(r.Value));
            }
            else if (obj is JsonElement json)
            {
                if (json.ValueKind != JsonValueKind.Object)
                    throw new InvalidCastException();

                return (JsonElement2Object(json) as IDictionary<string, object?>)!;
            }
            else
            {
                var props = objType
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);

                props.ForEachItem(propertyInfo =>
                {
                    if (propertyInfo.GetIndexParameters().Any() || !propertyInfo.CanRead) return;
                    var value = propertyInfo.GetValue(obj);
                    if (value is null || propertyInfo.PropertyType.IsSimpleType())
                        dic[propertyInfo.Name] = value;
                    else if (value is IDictionary || propertyInfo.PropertyType.IsSubTypeOfGenericType(typeof(IDictionary<,>)))
                        dic[propertyInfo.Name] = MapToDictionary(value);
                    else if (value is IEnumerable list)
                    {
                        dic[propertyInfo.Name] = list.OfType<object>().Select(ele =>
                        {
                            if (ele.GetType().IsSimpleType())
                                return ele;
                            return MapToDictionary(ele);
                        }).ToList();
                    }
                    else dic[propertyInfo.Name] = MapToDictionary(value);
                });

                return dic;
            }
        }

        /// <summary>
        /// 将对象属性转换为字典,复杂类型会递归转换,支持JsonElement/IDictionary/JsonObject等<para></para>
        /// 会将所有递归属性转换到根级属性<para></para>
        /// 例: new {User = new {Name = "lisi"}} : result["User.Name"] => "lisi"<para></para>
        /// 数组将转换为下表<para></para>
        /// 例: new {Users = { "zhangsan", "lisi"} } : result["Users.0"] => "zhangsan"<para></para>
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        public static IDictionary<string, object?> MapToRootDictionary<TModel>(this TModel obj)
        {
            var mapToDictionary = obj.MapToDictionary();
            var dictionary = new Dictionary<string, object?>();
            foreach (var item in mapToDictionary)
                MapToRootDictionaryFillChildren(dictionary, item.Value, item.Key);
            return dictionary;
        }

        /// <summary>
        /// 获取属性值，支持JsonElement/IDictionary/JsonObject
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="obj"></param>
        /// <param name="prop">属性名称, 子级属性.连接,例: User.Name</param>
        /// <returns></returns>
        public static (bool exists, object? value) GetPropertyValue<TModel>(this TModel obj, string prop)
        {
            if (obj is null) throw new ArgumentNullException(nameof(obj));
            if (string.IsNullOrWhiteSpace(prop))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(prop));

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
        /// 是否为类型<paramref name="parentType"/>的子类
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parentType">父类</param>
        /// <returns></returns>
        public static bool IsSubType(this Type type, Type parentType)
        {
            return type != parentType && parentType.IsAssignableFrom(type);
        }

        /// <summary>
        /// 是否为类型<typeparamref name="T"/>的子类
        /// </summary>
        /// <typeparam name="T">父类</typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSubType<T>(this Type type)
        {
            return type != typeof(T) && type.IsSubTypeOrEqualsOf(typeof(T));
        }

        /// <summary>
        /// 是否为类型<paramref name="parentType"/>的子类或自身
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parentType">父类</param>
        /// <returns></returns>
        public static bool IsSubTypeOrEqualsOf(this Type type, Type parentType)
        {
            return parentType.IsAssignableFrom(type);
        }

        /// <summary>
        /// 是否为类型<typeparamref name="T"/>的子类或自身
        /// </summary>
        /// <typeparam name="T">父类</typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSubTypeOrEqualsOf<T>(this Type type)
        {
            return type.IsSubTypeOrEqualsOf(typeof(T));
        }

        /// <summary>
        /// 对象深度克隆(json序列化/反序列化)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T? Clone<T>(this T? obj)
        {
            if (obj is not null)
                return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(obj));
            return default;
        }

        /// <summary>
        /// 是否定义了特性类型<typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">特性类型</typeparam>
        /// <param name="member"></param>
        /// <param name="inherit">是否继承</param>
        /// <returns></returns>
        public static bool IsDefinedAttribute<T>(this MemberInfo member, bool inherit)
            where T : Attribute
        {
            return Attribute.IsDefined(member, typeof(T), inherit);
        }

        /// <summary>
        /// 是否定义了特性特性类型<paramref name="type"/>
        /// </summary>
        /// <param name="member"></param>
        /// <param name="type">特性类型</param>
        /// <param name="inherit">是否继承</param>
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
                    return (typeof(TimeSpan) == type)
                        || (typeof(DateTimeOffset) == type)
                        || (typeof(DateOnly) == type)
                        || (typeof(TimeOnly) == type)
                        ;
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
            return type.IsSubTypeOrEqualsOf<IEnumerable>();
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
        /// 获取所有的父级类型,不包含object,非接口类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static HashSet<Type> GetAllBaseTypes(this Type type)
        {
            var types = new HashSet<Type>();
            FillBaseType(types, type);
            return types;
        }

        /// <summary>
        /// 获取实现的接口
        /// </summary>
        /// <param name="type"></param>
        /// <param name="includeBaseTypeInherited">是否包含父类实现的接口</param>
        /// <returns></returns>
        public static IEnumerable<Type> GetAllInterfaces(this Type type, bool includeBaseTypeInherited)
        {
            Func<Type, Type> select = r => r.IsGenericType && r.AssemblyQualifiedName is null ? r.GetGenericTypeDefinition() : r;

            if (includeBaseTypeInherited || type.BaseType is null)
                return type.GetInterfaces().Select(select);
            return type.GetInterfaces()
                .Select(select)
                .Except(type.BaseType.GetInterfaces().Select(select));
        }

        /// <summary>
        /// 获取所有的父级类型和接口类型,不包含object
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetAllBaseTypesAndInterfaces(this Type type)
        {
            return GetAllBaseTypes(type).Concat(GetAllInterfaces(type, true));
        }

        /// <summary>
        /// 属性值浅拷贝,常用于Action&lt;Options&gt;,属性值拷贝
        /// </summary>
        /// <param name="source">源对象</param>
        /// <param name="dest">目标对象</param>
        /// <param name="skipNullValue">是否跳过Null</param>
        /// <typeparam name="T"></typeparam>
        public static void CopyTo<T>(this T source, T dest, bool skipNullValue = false)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (dest is null) throw new ArgumentNullException(nameof(dest));

            var type = typeof(T);

            foreach (var propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public |
                                                            BindingFlags.GetProperty))
            {
                if (propertyInfo.GetIndexParameters().Any() || !propertyInfo.CanRead)
                    continue;
                var value = propertyInfo.GetValue(source);
                if (value is null && skipNullValue)
                    continue;
                propertyInfo.SetValue(dest, value);
            }
        }

        /// <summary>
        /// get json object property value and parse to type: <typeparamref name="T"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName">property name</param>
        /// <typeparam name="T">value type</typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static T? GetValue<T>(this JsonElement obj, string propertyName)
        {
            if (obj.ValueKind != JsonValueKind.Object)
                throw new ArgumentException("Json value kind must be JsonValueKind.Object");
            if (obj.TryGetProperty(propertyName, out var value))
            {
                switch (value.ValueKind)
                {
                    case JsonValueKind.Null:
                    case JsonValueKind.Undefined:
                        return default;
                }

                var res = GetValue(value, typeof(T));
                if (res is null) return default;
                return (T)res;
            }

            throw new ArgumentException($"Can not find json property \"{propertyName}\" in json {obj}");
        }

        /// <summary>
        /// get json object property value and parse to: <paramref name="type"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type">value type</param>
        /// <param name="propertyName">property name</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static object? GetValue(this JsonElement obj, Type type, string propertyName)
        {
            if (obj.ValueKind != JsonValueKind.Object)
                throw new ArgumentException("Json value kind must be JsonValueKind.Object");
            if (obj.TryGetProperty(propertyName, out var value))
                return GetValue(value, type);

            throw new ArgumentException($"Can not find json property \"{propertyName}\" in json {obj}");
        }

        /// <summary>
        /// get json value to type: <typeparamref name="T"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T? GetValue<T>(this JsonElement obj)
        {
            switch (obj.ValueKind)
            {
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:

                    return default;
            }

            var res = GetValue(obj, typeof(T));
            if (res is null) return default;

            return (T)res;
        }

        /// <summary>
        /// get json value to <paramref name="type"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        public static object? GetValue(this JsonElement obj, Type type)
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
                            Type? enumType = null;
                            if (type.IsEnum)
                                enumType = type;
                            else if (type.IsNullableType() && type.GetGenericArguments().First().IsEnum)
                                enumType = type.GetGenericArguments().First();

                            if (enumType is not null)
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

                        #region ->char

                        if (type == typeof(char) || type == typeof(char?))
                        {
                            if (obj.TryGetUInt16(out var value))
                            {
                                try
                                {
                                    return (char)value;
                                }
                                catch
                                {
                                    throw GetInvalidCastException(type, obj);
                                }
                            }
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

                        Type? enumType = null;
                        if (type.IsEnum)
                        {
                            enumType = type;
                        }
                        else if (type.IsNullableType() && type.GetGenericArguments().First().IsEnum)
                        {
                            enumType = type.GetGenericArguments().First();
                        }

                        if (enumType is not null)
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

                        #region ->char

                        if (type == typeof(char) || type == typeof(char?))
                        {
                            var str = obj.GetString();
                            if (str is null || str.Length == 0)
                                return GetDefaultValue(type);
                            if (str.Length == 1)
                                return str[0];

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

                        return JsonSerializer.Deserialize(bufferWriter.WrittenSpan, type);
                    }
                default:
                    throw GetInvalidCastException(type, obj);
            }
        }

        /// <summary>
        /// 获取枚举值描述,<see cref="DescriptionAttribute"/>特性值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string? GetEnumDescription(this Enum value)
        {
            var name = value.ToString();
            var fieldInfo = value.GetType().GetField(name);
            return fieldInfo?.GetCustomAttribute<DescriptionAttribute>()?.Description;
        }

        #region private

        private static readonly Dictionary<Type, object> CommonTypeDictionary = new Dictionary<Type, object>
        {
            {typeof(int), default(int)},
            {typeof(Guid), default(Guid)!},
            {typeof(DateTime), default(DateTime)!},
            {typeof(DateTimeOffset), default(DateTimeOffset)!},
            {typeof(long), default(long)},
            {typeof(bool), default(bool)},
            {typeof(double), default(double)},
            {typeof(short), default(short)},
            {typeof(float), default(float)},
            {typeof(byte), default(byte)},
            {typeof(char), default(char)},
            {typeof(uint), default(uint)},
            {typeof(ushort), default(ushort)},
            {typeof(ulong), default(ulong)},
            {typeof(sbyte), default(sbyte)}
        };

        private static object JToken2Object(JToken token)
        {
            if (token is null) throw new ArgumentException(nameof(token));

            switch (token.Type)
            {
                case JTokenType.Object:
                    {
                        var dic = new Dictionary<string, object>();
                        foreach (var item in token.Value<JObject>()!)
                        {
                            dic[item.Key] = JToken2Object(item.Value!);
                        }

                        return dic;
                    }

                case JTokenType.Array:
                    {
                        var children = new List<object>();
                        foreach (var item in token.Value<JArray>()!)
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
                    return token.ToObject<object>()!;
                default:
                    throw new JsonException($"Invalid json string: {token}");
            }
        }

        private static object? JTokenValue(JToken token)
        {
            if (token is null) return null;
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
                    throw new JsonException($"Invalid json string: {token}");
            }
        }

        private static object? JsonElement2Object(JsonElement obj)
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
                        if (obj.TryGetInt32(out var value1))
                            return value1;
                        if (obj.TryGetInt64(out var value2))
                            return value2;
                        if (obj.TryGetDecimal(out var value3))
                            return value3;

                        return null;
                    }
                case JsonValueKind.Object:
                    {
                        var dic = new Dictionary<string, object?>();
                        foreach (var item in obj.EnumerateObject())
                        {
                            dic[item.Name] = JsonElement2Object(item.Value);
                        }

                        return dic;
                    }
                case JsonValueKind.Array:
                    {
                        var children = new List<object?>();
                        foreach (var item in obj.EnumerateArray())
                            children.Add(JsonElement2Object(item));
                        return children;
                    }
                default:
                    throw new JsonException($"Invalid json string: {obj}");
            }
        }

        private static object? JsonElementValue(JsonElement obj)
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
                        if (obj.TryGetDateTimeOffset(out var datetimeOffset))
                            return datetimeOffset;
                        return obj.GetString();
                    }
                case JsonValueKind.True:
                case JsonValueKind.False:
                    return obj.GetBoolean();
                case JsonValueKind.Number:
                    {
                        if (obj.TryGetInt32(out var value1))
                            return value1;
                        if (obj.TryGetInt64(out var value2))
                            return value2;
                        if (obj.TryGetDecimal(out var value3))
                            return value3;

                        return null;
                    }
                case JsonValueKind.Object:
                case JsonValueKind.Array:
                    return obj;
                default:
                    throw new JsonException($"Invalid json string: {obj}");
            }
        }

        private static (bool exists, object? value) GetObjectValue(object obj, string proName)
        {
            var objType = obj.GetType();
            if (obj is JToken jToken)
            {
                JToken? json;
                if (jToken.Type == JTokenType.Array && int.TryParse(proName, out var index))
                {
                    var jsonArr = jToken.Value<JArray>()!;
                    if (jsonArr.Count < index + 1)
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

                if (json is not null)
                    return (true, JTokenValue(json));
                return (false, null);
            }
            else if (objType.IsSubTypeOfGenericType(typeof(IDictionary<,>)))
            {
                var list = (obj as IEnumerable)!.OfType<dynamic>();
                try
                {
                    var el = list.First(r => r.Key.ToString() == proName);
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
                JsonElement? json;
                if (jsonObj.ValueKind == JsonValueKind.Array && int.TryParse(proName, out var index))
                {
                    if (jsonObj.GetArrayLength() < index + 1)
                    {
                        return (false, null);
                    }

                    json = jsonObj[index];
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

                return (true, JsonElementValue(json.Value));
            }
            else if (obj is IEnumerable list && int.TryParse(proName, out var index))
            {
                var i = 0;
                foreach (var item in list)
                {
                    if (i == index)
                        return (true, item);
                    i++;
                }

                return (false, null);
            }
            else
            {
                var pro = objType.GetProperty(proName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
                if (pro is null) return (false, null);
                return (true, pro.GetValue(obj));
            }
        }

        private static void FillBaseType(HashSet<Type> results, Type type)
        {
            if (type.BaseType == null)
                return;
            if (type.BaseType == typeof(object))
                return;
            if (type.BaseType.IsGenericType && type.BaseType.AssemblyQualifiedName == null)
            {
                var genericTypeDefinition = type.BaseType.GetGenericTypeDefinition();
                results.Add(genericTypeDefinition);
                FillBaseType(results, genericTypeDefinition);
            }
            else
            {
                results.Add(type.BaseType);
                FillBaseType(results, type.BaseType);
            }
        }

        private static InvalidCastException GetInvalidCastException(Type type, JsonElement json)
        {
            return new InvalidCastException($"Can not convert to type:{type} from json value: {json}");
        }

        private static void MapToRootDictionaryFillChildren(IDictionary<string, object?> dic, object? obj, string prefix)
        {
            if (obj is null || obj.GetType().IsSimpleType())
                dic[prefix] = obj;
            else if (obj.GetType().IsSubTypeOfGenericType(typeof(IDictionary<,>)))
            {
                ((IEnumerable)obj)
                    .OfType<dynamic>()
                    .ForEachItem(item =>
                    {
                        if (item.Value is null || IsSimpleType(item.Value.GetType()))
                            dic[prefix + "." + item.Key] = item.Value;
                        MapToRootDictionaryFillChildren(dic, item.Value, prefix + "." + item.Key);
                    });
            }
            else if (obj is IDictionary dictionary)
            {
                foreach (DictionaryEntry item in dictionary)
                {
                    if (item.Value is null || item.Value.GetType().IsSimpleType())
                        dic[prefix + "." + item.Key] = item.Value;
                    MapToRootDictionaryFillChildren(dic, item.Value, prefix + "." + item.Key);
                }
            }
            else if (obj is IEnumerable list)
            {
                int index = 0;
                foreach (var item in list)
                {
                    if (item is null || item.GetType().IsSimpleType())
                        dic[$"{prefix}.{index++}"] = item;
                    else MapToRootDictionaryFillChildren(dic, item, $"{prefix}.{index++}");
                }
            }
        }

        #endregion
    }
}