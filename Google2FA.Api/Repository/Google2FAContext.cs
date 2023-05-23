using Microsoft.EntityFrameworkCore;
using UserApi.Models;

namespace UserApi.Repository
{
    public class Google2FAContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: "UserDb");
        }

        public DbSet<User> Users { get; set; }
    }
}
