
using FucoMicro.Services.RewardAPI.Message;

namespace FucoMicro.Services.RewardAPI.Services
{
    public interface IRewardService
    {
        Task UpdateRewards(RewardsMessage rewardsMessage);
    }
}
