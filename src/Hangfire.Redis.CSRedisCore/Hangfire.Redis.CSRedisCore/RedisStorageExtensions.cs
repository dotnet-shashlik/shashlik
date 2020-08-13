// Copyright � 2013-2015 Sergey Odinokov, Marco Casamento 
// This software is based on https://github.com/HangfireIO/Hangfire.Redis 

// Hangfire.Redis.StackExchange is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as 
// published by the Free Software Foundation, either version 3 
// of the License, or any later version.
// 
// Hangfire.Redis.StackExchange is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public 
// License along with Hangfire.Redis.StackExchange. If not, see <http://www.gnu.org/licenses/>.

using System;
using Hangfire.Annotations;
using Hangfire.Redis;
namespace Hangfire
{
    public static class RedisStorageExtensions
    {
        /// <summary>
        /// 使用CSRedis 作为hangfire存储
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="redisClient"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IGlobalConfiguration<RedisStorage> UseRedisStorage(
            [NotNull] this IGlobalConfiguration configuration, CSRedis.CSRedisClient redisClient, RedisStorageOptions options = null)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            var storage = new RedisStorage(redisClient, options);
            return configuration.UseStorage(storage);
        }
    }
}