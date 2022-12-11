using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace Shashlik.EfCore.Json;

/// <summary>
/// json存储转换器
/// </summary>
/// <typeparam name="T"></typeparam>
public class JsonValueConverter<T> : ValueConverter<T, string> where T : class
{
    public JsonValueConverter() :
        base(v => JsonConvert.SerializeObject(v),
            v => JsonConvert.DeserializeObject<T>(v)!)
    {
    }
}