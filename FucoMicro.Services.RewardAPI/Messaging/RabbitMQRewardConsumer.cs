using FucoMicro.Services.RewardAPI.Message;
using FucoMicro.Services.RewardAPI.Services;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace FucoMicro.Services.RewardAPI.Messaging
{
    public class RabbitMQRewardConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly RewardService _rewardService;
        private IConnection _connection;
        private IChannel _channel;
        private string queueName = "";

        public RabbitMQRewardConsumer(IConfiguration configuration, RewardService rewardService)
        {
            _configuration = configuration;
            _rewardService = rewardService;

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Password = "guest",
                UserName = "guest"
            };

            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
            _channel.ExchangeDeclareAsync(_configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic"), 
                ExchangeType.Fanout, durable: false).GetAwaiter().GetResult();

            queueName = _channel.QueueDeclareAsync().GetAwaiter().GetResult().QueueName;
            _channel.QueueBindAsync(queueName, _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic"), "");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += (ch, ev) =>
            {
                var content = Encoding.UTF8.GetString(ev.Body.ToArray());

                RewardsMessage rewardsMessage = JsonConvert.DeserializeObject<RewardsMessage>(content);

                HandleMessage(rewardsMessage).GetAwaiter().GetResult();

                _channel.BasicAckAsync(ev.DeliveryTag, false);

                return Task.CompletedTask;
            };

            _channel.BasicConsumeAsync(queueName, false, consumer).GetAwaiter().GetResult();

            return Task.CompletedTask;
        }

        private async Task HandleMessage(RewardsMessage rewardsMessage)
        {
            await _rewardService.UpdateRewards(rewardsMessage);
        }
    }
}
