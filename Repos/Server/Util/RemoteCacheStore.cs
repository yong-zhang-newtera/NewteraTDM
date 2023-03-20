using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Newtera.Common.Core;

namespace Newtera.Server.Util
{
    /// <summary>
    /// KeyValueStore class using Redis
    /// </summary>
    public class RemoteCacheStore : IKeyValueStore
    {
        private IMasterServerClient _cacheClient = null;
        private object _clientLock = new object();
        private string _cacheName;
      
        public RemoteCacheStore(IMasterServerClient cacheClient, string cacheName)
        {
            this._cacheClient = cacheClient;
            this._cacheName = cacheName;
        }

        public void Initialize<T>(IDictionary<string, T> keyValues)
        {
            foreach (string key in keyValues.Keys)
            {
                this._cacheClient.Add<T>(this._cacheName, key, keyValues[key]);
            }
        }

        public void Add<T>(string key, T value)
        {
            this._cacheClient.Add<T>(this._cacheName, key, value);
        }

        public bool Contains(string key)
        {
            return this._cacheClient.Contains(this._cacheName, key);
        }

        public T Get<T>(string key)
        {
            if (this._cacheClient.Contains(this._cacheName, key))
            {
                var value = this._cacheClient.Get<T>(this._cacheName, key);
                return value;
            }
            else
            {
                return default(T);
            }

        }

        public void Remove(string key)
        {
            if (this._cacheClient.Contains(this._cacheName, key))
            {
                this._cacheClient.Remove(this._cacheName, key);
            }
        }

        public IList<string> Keys
        {
            get
            {
                return this._cacheClient.GetKeys(this._cacheName);
            }
        }

        public IList<object> Values
        {
            get
            {
                return this._cacheClient.GetValues<object>(this._cacheName);
            }
        }

        public void Clear()
        {
            this._cacheClient.Clear(this._cacheName);
        }
    }
}
