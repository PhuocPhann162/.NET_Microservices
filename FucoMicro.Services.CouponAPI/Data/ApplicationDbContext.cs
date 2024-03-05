using FucoMicro.Services.CouponAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FucoMicro.Services.CouponAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base (options) 
        {

        }

        public DbSet<Coupon> Coupons { get; set; }
    }
}
