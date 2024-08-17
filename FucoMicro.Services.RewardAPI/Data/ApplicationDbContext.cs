using FucoMicro.Services.RewardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FucoMicro.Services.RewardAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<Rewards> Rewards { get; set; }
    }
}
