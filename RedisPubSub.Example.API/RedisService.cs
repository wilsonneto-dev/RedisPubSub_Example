using Newtonsoft.Json;
using StackExchange.Redis;

namespace RedisPubSub.Example.API
{
    public class RedisService : RedisConnectionBase, IMessageBusService
    {
        private readonly ILogger _logger;
        public RedisService() : base() { }

        public async Task<int> Publish(object obj)
        {
            return (int) await RedisCache.Multiplexer
                .GetSubscriber()
                .PublishAsync("catalog.items", JsonConvert.SerializeObject(obj));
        }

        public async Task Subscribe(string pattern, Action<RedisChannel, RedisValue> handler)
        {
            var channelPattern = new RedisChannel(pattern, RedisChannel.PatternMode.Pattern);
            await RedisCache.Multiplexer.GetSubscriber().SubscribeAsync(channelPattern, handler);
        }
    }

    public abstract class RedisConnectionBase
    {
        public RedisConnectionBase()
            => LazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect("localhost,port: 6379,password=Redis@2022!"));
        private readonly Lazy<ConnectionMultiplexer> LazyConnection;
        protected ConnectionMultiplexer Connection => LazyConnection.Value;
        protected IDatabase RedisCache => Connection.GetDatabase();
    }

    public interface IMessageBusService
    {
        Task<int> Publish(object order);
        Task Subscribe(string pattern, Action<RedisChannel, RedisValue> handler);
    }

    public static class RedisMessageBusServicesExtension
    {
        public static IServiceCollection AddRedisMessageBusService(this IServiceCollection services)
            => services.AddSingleton<IMessageBusService, RedisService>();
    }
}
