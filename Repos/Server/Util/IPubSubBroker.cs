using StackExchange.Redis;
using System;

namespace Newtera.Server.Util
{
    public interface IPubSubBroker
    {
        void Publish(string channelName, string message);

        ChannelMessageQueue Subscribe(string channelName);
    }
}
