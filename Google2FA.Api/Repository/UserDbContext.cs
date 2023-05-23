using Microsoft.EntityFrameworkCore;
using System;
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
                    TwoFAStatus = true
                },
                new User
                {
                    UserId = 2,
                    Username = "Shahzeb",
                    Password = "12345",
                    TwoFAStatus = true
                }
            );
        }
    }
}
