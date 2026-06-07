using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace comentapp_authentication_manager.Data
{
    public class ComentappDbContext : DbContext, IDataProtectionKeyContext
    {
        public ComentappDbContext(DbContextOptions<ComentappDbContext> options)
        : base(options)
        {
        }

        public DbSet<Models.User> Users { get; set; }
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
        //public DbSet<Models.UserCredentials> UserCredentials { get; set; }
    }
}
