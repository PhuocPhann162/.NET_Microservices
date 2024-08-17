using Azure.Messaging.ServiceBus;
using FucoMicro.Services.RewardAPI.Message;
using FucoMicro.Services.RewardAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace FucoMicro.Services.RewardAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string orderCreatedTopic;
        private readonly string orderCreatedRewardsSubcription;
        private readonly IConfiguration _configuration;
        private readonly RewardService _rewardService;

        private ServiceBusProcessor _rewardProcessor;

        public AzureServiceBusConsumer(IConfiguration configuration, RewardService rewardService)
        {
            _configuration = configuration;
            _rewardService = rewardService;

            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            orderCreatedTopic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
            orderCreatedRewardsSubcription = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_Rewards_Subscription");

            var client = new ServiceBusClient(serviceBusConnectionString);
            _rewardProcessor = client.CreateProcessor(orderCreatedTopic, orderCreatedRewardsSubcription);
        }

        public async Task Start()
        {
            _rewardProcessor.ProcessMessageAsync += OnNewOrderRewardsRequestReceived;
            _rewardProcessor.ProcessErrorAsync += ErrorHandler;
            await _rewardProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _rewardProcessor.StopProcessingAsync();
            await _rewardProcessor.DisposeAsync();
        }

        private async Task OnNewOrderRewardsRequestReceived(ProcessMessageEventArgs args)
        {
            // this is where you will receive the message 
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            RewardsMessage objMessage = JsonConvert.DeserializeObject<RewardsMessage>(body);

            try
            {
                await _rewardService.UpdateRewards(objMessage);
                await args.CompleteMessageAsync(message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
