
using FucoMicro.Services.EmailAPI.Services;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace FucoMicro.Services.EmailAPI.Messaging
{
    public class RabbitMQAuthConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private IConnection _connection;
        private IChannel _channel;

        public RabbitMQAuthConsumer(IConfiguration configuration, EmailService emailService)
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
            _channel.QueueDeclareAsync(_configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"), 
                false, false, false, null).GetAwaiter().GetResult();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += (ch, ev) =>
            {
                var content = Encoding.UTF8.GetString(ev.Body.ToArray());

                String email = JsonConvert.DeserializeObject<string>(content);

                HandleMessage(email).GetAwaiter().GetResult();

                _channel.BasicAckAsync(ev.DeliveryTag, false);

                return Task.CompletedTask;
            };

            _channel.BasicConsumeAsync(_configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"), false, consumer).GetAwaiter().GetResult();

            return Task.CompletedTask;
        }

        private async Task HandleMessage(string email)
        {
            await _emailService.RegisterUserEmailAndLog(email);
        }
    }
}
