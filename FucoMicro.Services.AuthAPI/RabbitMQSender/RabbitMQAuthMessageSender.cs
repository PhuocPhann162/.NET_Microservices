using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace FucoMicro.Services.AuthAPI.RabbitMQSender
{
    public class RabbitMQAuthMessageSender : IRabbitMQAuthMessageSender
    {
        private readonly string _hostName;
        private readonly string _username;
        private readonly string _password;
        private IConnection _connection;

        public RabbitMQAuthMessageSender()
        {
            _hostName = "localhost";
            _username = "guest";
            _password = "guest";
        }

        public void SendMessage(object message, string queueName)
        {
            if (ConnectionExists())
            {
                using var channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

                channel.QueueDeclareAsync(queueName, false, false, false, null).GetAwaiter().GetResult();

                var json = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(json);

                channel.BasicPublishAsync(exchange: "", routingKey: queueName, body: body);
            }

        }

        private void CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _hostName,
                    UserName = _username,
                    Password = _password,
                };

                _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private bool ConnectionExists()
        {
            if (_connection == null)
            {
                return true;
            }
            CreateConnection();
            return true;
        }
    }
}
