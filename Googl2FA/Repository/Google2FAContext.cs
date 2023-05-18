using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;

namespace Googl2FA.Repository
{
    public class Google2FAContext : DbContext
    {
        protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: "UserDb");
        }

        public DbSet<User> Users { get; set; }
    }
}
