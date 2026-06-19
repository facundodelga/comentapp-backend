using comentapp.persistence.Models;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace comentapp.persistence
{
    public class ComentappDbContext : DbContext, IDataProtectionKeyContext
    {
        public ComentappDbContext(DbContextOptions<ComentappDbContext> options)
        : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Setting> Settings { get; set; }
        //public DbSet<Models.UserCredentials> UserCredentials { get; set; }
    }
}
