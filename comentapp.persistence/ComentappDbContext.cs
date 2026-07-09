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
        public DbSet<Creator> Creators { get; set; }
        public DbSet<Comment> Comments { get; set; }


        //public DbSet<Models.UserCredentials> UserCredentials { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Creator>(entity =>
            {
                entity.HasOne(c => c.User)
                    .WithOne(u => u.Creator)
                    .HasForeignKey<Creator>(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Relación Comment -> User
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasOne(c => c.User)
                    .WithMany(u => u.Comments)
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Relación Comment -> Creator
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasOne(c => c.Creator)
                    .WithMany(cr => cr.Comments)
                    .HasForeignKey(c => c.CreatorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Relación Payment -> User
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasOne(p => p.User)
                    .WithMany(u => u.Payments)
                    .HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Relación Payment -> Creator
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasOne(p => p.Creator)
                    .WithMany(cr => cr.Payments)
                    .HasForeignKey(p => p.CreatorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Relación Payment -> Comentario (1:1, solo desde Payment)
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasOne(p => p.Comment)
                    .WithOne()  // ✅ Sin WithOne(c => c.Pago)
                    .HasForeignKey<Comment>(c => c.PaymentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
