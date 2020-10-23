using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CSRedis;
using Microsoft.AspNetCore.DataProtection.Repositories;
// ReSharper disable CheckNamespace

namespace Shashlik.DataProtection
{
    /// <summary>
    /// An XML repository backed by a Redis list entry.
    /// </summary>
    public class RedisXmlRepository : IXmlRepository
    {
        private CSRedisClient RedisClient { get; }

        private string Key { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redisClient"></param>
        /// <param name="key"></param>
        public RedisXmlRepository(CSRedisClient redisClient, string key)
        {
            RedisClient = redisClient;
            Key = key;
        }

        /// <inheritdoc />
        public IReadOnlyCollection<XElement> GetAllElements()
        {
            return GetAllElementsCore().ToList().AsReadOnly();
        }

        private IEnumerable<XElement> GetAllElementsCore()
        {
            // Note: Inability to read any value is considered a fatal error (since the file may contain
            // revocation information), and we'll fail the entire operation rather than return a partial
            // set of elements. If a value contains well-formed XML but its contents are meaningless, we
            // won't fail that operation here. The caller is responsible for failing as appropriate given
            // that scenario.
            return RedisClient.LRange(Key, 0, -1).Select(XElement.Parse);
        }

        /// <inheritdoc />
        public void StoreElement(XElement element, string friendlyName)
        {
            // var database = _databaseFactory();
            // database.ListRightPush();
            RedisClient.RPush(Key, element.ToString(SaveOptions.DisableFormatting));
        }
    }
}