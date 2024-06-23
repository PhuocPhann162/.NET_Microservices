namespace FucoMicro.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;
        private readonly IConfiguration _configuration;

        public AzureServiceBusConsumer(IConfiguration configuration)
        {
            _configuration = configuration;

            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
        }
    }
}
