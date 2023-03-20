using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newtera.Test
{
    public struct KeyValue<K, V>
    {
        public K Key { get; set; }
        public V Value { get; set; }
    }

    public class FixedSizeHashTable<K, V>
    {
        private readonly int size;
        private LinkedList<KeyValue<K, V>>[] list;

        public FixedSizeHashTable(int size)
        {
            this.size = size;
            this.list = new LinkedList<KeyValue<K, V>>[this.size];
        }

        private int GetArrayPosition(K key)
        {
            int pos = key.GetHashCode() % this.size;
            return Math.Abs(pos);
        }

        public V Find(K key)
        {
            int pos = GetArrayPosition(key);

            LinkedList<KeyValue<K, V>> subList = GetSubList(pos);

            foreach (KeyValue<K, V> kv in subList)
            {
                if (kv.Key.Equals(key))
                {
                    return kv.Value;
                }
            }

            return default(V);
        }

        public void Add(K key, V value)
        {
            int pos = GetArrayPosition(key);

            LinkedList<KeyValue<K, V>> subList = GetSubList(pos);

            subList.AddLast(new KeyValue<K, V>() { Key = key, Value = value });
        }

        public void Delete(K key)
        {
            int pos = GetArrayPosition(key);

            LinkedList<KeyValue<K, V>> subList = GetSubList(pos);
            bool isFound = false;
            KeyValue<K, V> found = default(KeyValue<K, V>);
            foreach (KeyValue<K, V> kv in subList)
            {
                if (kv.Key.Equals(key))
                {
                    isFound = true;
                    found = kv;
                    break;
                }
            }

            if (isFound)
            {
                subList.Remove(found);
            }
        }

        public int FindNum(List<int> arr)
        {
            int size = arr.Count;

            if (arr == null || arr.Count == 0)
                throw new Exception("Empty array");

            if (arr.Count == 1)
                return arr[0];

            Dictionary<int, int> map = new Dictionary<int, int>();

            for (int i = 0; i < size; i++)
            {
                if (map.ContainsKey(arr[i]))
                {
                    map[arr[i]]++;
                }
                else
                {
                    map[arr[i]] = 1;
                }

                if (map[arr[i]] > size / 2)
                {
                    return arr[i];
                }
            }

            throw new Exception("No num");
        }

        private LinkedList<KeyValue<K, V>> GetSubList(int pos)
        {
            if (this.list[pos] == null)
            {
                this.list[pos] = new LinkedList<KeyValue<K, V>>();
            }

            return this.list[pos];
        }
    }
}
