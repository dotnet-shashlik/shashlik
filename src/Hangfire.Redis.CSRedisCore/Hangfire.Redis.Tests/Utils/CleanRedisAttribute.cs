﻿using System.Reflection;
using System.Threading;
using Xunit;
using Xunit.Sdk;

namespace Hangfire.Redis.Tests
{
    public class CleanRedisAttribute : BeforeAfterTestAttribute
    {
        private static readonly object GlobalLock = new object();

        public override void Before(MethodInfo methodUnderTest)
        {
            Monitor.Enter(GlobalLock);
            var client = RedisUtils.RedisClient;
            client.NodesServerManager.FlushDb();
            //client.Multiplexer.GetServer(RedisUtils.GetHostAndPort()).FlushDatabase(RedisUtils.GetDb());
        }

        public override void After(MethodInfo methodUnderTest)
        {
            if (Monitor.IsEntered(GlobalLock))
                Monitor.Exit(GlobalLock);
        }
    }
}
