using System;
using Newtera.Common.Config;
using StackExchange.Redis;

namespace Newtera.Server.Util
{
    public class RedisPubSubBroker : IPubSubBroker
    {
        ConnectionMultiplexer _redis;

        public void Publish(string channelName, string message)
        {
            var subscriber = GetSubscriber();
            subscriber.Publish(channelName, message);
        }

        public ChannelMessageQueue Subscribe(string channelName)
        {
            var subscriber = GetSubscriber();
            return subscriber.Subscribe(channelName);
        }

        private ISubscriber GetSubscriber()
        {
            if (this._redis is null)
            {
                string redisConnectionString = RedisConfig.Instance.ConnectionString;
                _redis = ConnectionMultiplexer.Connect(redisConnectionString);
            }

            return this._redis.GetSubscriber();
        }
    }
}
