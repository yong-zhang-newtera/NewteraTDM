using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newtera.Server.Util
{
    /// <summary>
    /// KeyValueStore class using Hashtable
    /// </summary>
    public class HashtableStore : IKeyValueStore
    {
        private object internalLock = new object();
        private string _tableName;
        private Dictionary<string, object> _cache;
        public HashtableStore(string tableName)
        {
            this._tableName = tableName;
            this._cache = new Dictionary<string, object>();
        }

        public void Initialize<T>(IDictionary<string, T> keyValues)
        {
            foreach (string key in keyValues.Keys)
            {
                this._cache.Add(key, keyValues[key]);
            }
        }

        public void Add<T>(string key, T value)
        {
            lock (this.internalLock)
            {
                this._cache.Add(key, value);
            }
        }

        public bool Contains(string key)
        {
            lock (this.internalLock)
            {
                return this._cache.ContainsKey(key);
            }
        }

        public T Get<T>(string key)
        {
            lock (this.internalLock)
            {
                if (this._cache.ContainsKey(key))
                    return (T)this._cache[key];
                else
                    return default(T);
            }
        }

        public void Remove(string key)
        {
            lock (this.internalLock)
            {
                if (this._cache.ContainsKey(key))
                    this._cache.Remove(key);
            }
        }

        public IEnumerable<string> GetKeys()
        {
            lock (this.internalLock)
            {
                return this._cache.Keys.ToList<string>();
            }
        }

        public void Clear()
        {
            lock (this.internalLock)
            {
                this._cache.Clear();
            }
        }
    }
}
