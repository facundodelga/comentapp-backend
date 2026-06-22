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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Creator>(entity =>
            {
                entity.ToTable("Creators");

                entity.HasKey(c => c.Id);

                entity.Property(c => c.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(c => c.CreatorName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(c => c.UserId)
                    .IsRequired();

                entity.Property(c => c.MercadoPagoAccount)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(c => c.InstagramLink)
                    .HasMaxLength(300);

                entity.Property(c => c.TikTokLink)
                    .HasMaxLength(300);

                entity.Property(c => c.YouTubeLink)
                    .HasMaxLength(300);

                entity.Property(c => c.TwitchLink)
                    .HasMaxLength(300);

                entity.Property(c => c.KickLink)
                    .HasMaxLength(300);

                entity.Property(c => c.Description)
                    .HasMaxLength(1000);

                entity.HasOne(c => c.User)
                    .WithOne()
                    .HasForeignKey<Creator>(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(c => c.UserId)
                    .IsUnique();

                entity.HasIndex(c => c.CreatorName)
                    .IsUnique();
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("Comments");

                entity.HasKey(c => c.Id);

                entity.Property(c => c.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(c => c.CommentText)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(c => c.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(c => c.UserId)
                    .IsRequired();

                entity.Property(c => c.CreatorId)
                    .IsRequired();

                entity.HasOne(c => c.User)
                    .WithMany()
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Creator)
                    .WithMany()
                    .HasForeignKey(c => c.CreatorId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(c => c.UserId);

                entity.HasIndex(c => c.CreatorId);
            });
        }
    }
}
