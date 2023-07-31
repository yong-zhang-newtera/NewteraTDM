using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtera.Common.Config;

namespace Newtera.Server.Util
{
    public class KeyValueStoreFactory
    {
        private static KeyValueStoreFactory theInstance = null;
        private static object internalLock = new object();

        private KeyValueStoreFactory()
        {
        }

        public static KeyValueStoreFactory TheInstance
        {
            get
            {
                if (theInstance == null)
                {
                    lock (internalLock)
                    {
                        if (theInstance == null)
                            theInstance = new KeyValueStoreFactory();
                    }
                }

                return theInstance;
            }
        }

        public bool UseDistributedCache
        {
            get
            {
                return RedisConfig.Instance.DistributedCacheEnabled;
            }
        }

        public IKeyValueStore Create(string cacheName)
        {
            if (UseDistributedCache)
                return new RedisCache(cacheName);
            else
                return new HashtableStore(cacheName);
        }
    }
}
