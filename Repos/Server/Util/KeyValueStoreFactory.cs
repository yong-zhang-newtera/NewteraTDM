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
        private IMasterServerClient _masterServerClient;

        private static KeyValueStoreFactory theInstance = null;
        private static object internalLock = new object();

        private KeyValueStoreFactory()
        {
            _masterServerClient = null;
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

        public bool UseRemoteCacheStore
        {
            get
            {
                return false;
                //return ClusterServerConfig.Instance.MoreThanOneServers;
            }
        }

        public IMasterServerClient MasterServerClient
        {
            get
            {
                return this._masterServerClient;
            }
            set
            {
                this._masterServerClient = value;
            }
        }

        public IKeyValueStore Create(string tableName)
        {
            if (UseRemoteCacheStore)
                return new RemoteCacheStore(MasterServerClient, tableName);
            else
                return new HashtableStore(tableName);
        }
    }
}
