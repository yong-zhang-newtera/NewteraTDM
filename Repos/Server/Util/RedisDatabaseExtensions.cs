using System;
using System.Threading.Tasks;

using Newtonsoft.Json;

using StackExchange.Redis;

namespace Newtera.Server.Util
{
    public static class RedisDatabaseExtensions
    {
        public static async Task SetRecordAsync<T>(this IDatabase database,
            string recordId,
            T data)
        {
            var jsonData = JsonConvert.SerializeObject(data);
            await database.StringSetAsync(recordId, jsonData);
        }

        public static async Task<T> GetRecordAsync<T>(this IDatabase database,
            string recordId)
        {
            var redisValue = await database.StringGetAsync(recordId);

            if (redisValue.IsNull)
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(redisValue.ToString());
        }
    }
}
