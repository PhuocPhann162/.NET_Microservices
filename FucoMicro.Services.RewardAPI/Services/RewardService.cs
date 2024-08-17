using FucoMicro.Services.RewardAPI.Data;
using FucoMicro.Services.RewardAPI.Message;
using FucoMicro.Services.RewardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FucoMicro.Services.RewardAPI.Services
{
    public class RewardService : IRewardService
    {
        private DbContextOptions<ApplicationDbContext> _dbOptions;

        public RewardService(DbContextOptions<ApplicationDbContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }

        public async Task UpdateRewards(RewardsMessage rewardsMessage)
        {
            try
            {
                Rewards rewards = new()
                {
                    OrderId = rewardsMessage.OrderId,
                    RewardsActivity = rewardsMessage.RewardsActivity,
                    UserId = rewardsMessage.UserId,
                    RewardsDate = DateTime.Now,
                };
                await using var _db = new ApplicationDbContext(_dbOptions);
                await _db.Rewards.AddAsync(rewards);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
