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
    /// The remote cache client interface
    /// </summary>
    public interface IMasterServerClient
    {
        void Add<T>(string cacheName, string key, T value);

        bool Contains(string cacheName, string key);

        T Get<T>(string cacheName, string key);

        void Remove(string cacheName, string key);

        IList<string> GetKeys(string cacheNmae);

        IList<T> GetValues<T>(string cacheNmae);

        void Clear(string cacheName);
    }
}
