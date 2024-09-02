
using FucoMicro.Services.EmailAPI.Message;
using FucoMicro.Services.EmailAPI.Models.Dto;
using FucoMicro.Services.EmailAPI.Services;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace FucoMicro.Services.EmailAPI.Messaging
{
    public class RabbitMQOrderConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private IConnection _connection;
        private IChannel _channel;
        private string queueName = "";

        public RabbitMQOrderConsumer(IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;

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
            await _emailService.LogOrderPlaced(rewardsMessage);
        }
    }
}
