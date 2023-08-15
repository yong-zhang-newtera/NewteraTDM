using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StackExchange.Redis;

using Newtera.Common.Config;

namespace Newtera.Server.Util
{
    /// <summary>
    /// KeyValueStore class using Redis
    /// </summary>
    public class RedisCache : IKeyValueStore
    {
        private string _cacheName;
        ConnectionMultiplexer _redis;

        public RedisCache(string cacheName)
        {
            this._cacheName = cacheName;
            this._redis = null;
        }

        public void Initialize<T>(IDictionary<string, T> keyValues)
        {
            var database = GetDatabase();
            foreach (string key in keyValues.Keys)
            {
                database.SetRecordAsync<T>(BuildKey(key), keyValues[key]).GetAwaiter().GetResult();
            }
        }

        public void Add<T>(string key, T value)
        {
            var database = GetDatabase();

            database.SetRecordAsync<T>(BuildKey(key), value).GetAwaiter().GetResult();
        }

        public bool Contains(string key)
        {
            IDatabase database = GetDatabase();

            return database.KeyExistsAsync(BuildKey(key)).GetAwaiter().GetResult();
        }

        public T Get<T>(string key)
        {
            IDatabase database = GetDatabase();

            return database.GetRecordAsync<T>(BuildKey(key)).GetAwaiter().GetResult();
        }

        public void Remove(string key)
        {
            var database = GetDatabase();

            database.KeyDeleteAsync(BuildKey(key)).GetAwaiter().GetResult();
        }

        public IEnumerable<string> GetKeys()
        {
            List<string> cacheKeys = new List<string>();
 
            foreach (var ep in _redis.GetEndPoints())
            {
                var server = _redis.GetServer(ep);
                var keys = server.Keys(pattern: $"{KeyPrefix()}*");
                var keyPrefixLength = KeyPrefix().Length;
                foreach (var key in keys)
                {
                    var recordId = key.ToString().Substring(keyPrefixLength);
                    if (!cacheKeys.Contains(recordId))
                    {
                        cacheKeys.Add(recordId);
                    }
                }
            }

            return cacheKeys;
        }

        public void Clear()
        {
            var database = GetDatabase();

            var keys = this.GetKeys();
            foreach (string key in keys)
            {
                database.KeyDeleteAsync(key).GetAwaiter().GetResult();
            }
        }

        private IDatabase GetDatabase()
        {
            if (this._redis is null)
            {
                string redisConnectionString = RedisConfig.Instance.ConnectionString;
                _redis = ConnectionMultiplexer.Connect(redisConnectionString);
            }

            return this._redis.GetDatabase();
        }

        private string BuildKey(string recordId)
        {
            return $"{KeyPrefix()}{recordId}";
        }

        private string KeyPrefix()
        {
            return $"Newtera-{this._cacheName}-";
        }
    }
}
