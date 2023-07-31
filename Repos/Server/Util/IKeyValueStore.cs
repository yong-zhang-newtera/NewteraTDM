using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newtera.Server.Util
{
    public interface IKeyValueStore
    {
        void Initialize<T>(IDictionary<string, T> keyValues);

        void Add<T>(string key, T value);

        bool Contains(string key);

        T Get<T>(string key);

        void Remove(string key);

        IEnumerable<string> GetKeys();

        void Clear();
    }
}
