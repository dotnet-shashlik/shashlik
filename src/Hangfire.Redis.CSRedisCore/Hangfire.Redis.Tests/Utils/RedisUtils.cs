using System;

namespace Hangfire.Redis.Tests
{
    public static class RedisUtils
    {

        /// <summary>
        /// 连接字符串
        /// </summary>
        public const string ConnectionString = "192.168.50.178:6380,defaultDatabase=5,poolsize=50";

        public static CSRedis.CSRedisClient RedisClient { get; }

        static RedisUtils()
        {
            RedisClient = new CSRedis.CSRedisClient(ConnectionString, new string[] { }, false);
            RedisHelper.Initialization(RedisClient);
        }
        //public static CSRedis.CSRedisClient CreateClient()
        //{
        //    return RedisClient;
        //}
        //public static ISubscriber CreateSubscriber()
        //{
        //    return connection.Value.GetSubscriber();
        //}
        //public static string GetHostAndPort()
        //{
        //    return String.Format("{0}:{1}", GetHost(), GetPort());
        //}

        //public static string GetHost()
        //{
        //    return Environment.GetEnvironmentVariable(HostVariable)
        //           ?? DefaultHost;
        //}

        //public static int GetPort()
        //{
        //    var portValue = Environment.GetEnvironmentVariable(PortVariable);
        //    return portValue != null ? int.Parse(portValue) : DefaultPort;
        //}

        //public static int GetDb()
        //{
        //    var dbValue = Environment.GetEnvironmentVariable(DbVariable);
        //    return dbValue != null ? int.Parse(dbValue) : DefaultDb;
        //}
    }
}
