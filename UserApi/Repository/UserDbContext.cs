using Microsoft.EntityFrameworkCore;
using UserApi.Models;

namespace UserApi.Repository
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    Username = "Admin",
                    Password = "12345",
                    SecretKey = "E09Sn6gcC0p2lqG",
                    TwoFAStatus = true
                },
                new User
                {
                    UserId = 2,
                    Username = "Shahzeb",
                    Password = "12345",
                    SecretKey = "5Z0gsOxftKeCz2f",
                    TwoFAStatus = false
                }
            );
        }
    }
}
