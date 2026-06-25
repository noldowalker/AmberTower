using AuthService.Domain;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Persistence;

public sealed class AuthDbContext : DbContext
{
    public const string ConnectionStringName = "AuthDatabase";

    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }

    public DbSet<AuthUser> Users => Set<AuthUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuthUser>(entity =>
        {
            entity.ToTable("auth_users");
            entity.HasKey(user => user.Id);

            entity.Property(user => user.Email)
                .HasMaxLength(256)
                .IsRequired();

            entity.Property(user => user.PasswordHash)
                .HasMaxLength(512)
                .IsRequired();

            entity.Property(user => user.CreatedAtUtc)
                .IsRequired();

            entity.HasIndex(user => user.Email)
                .IsUnique();
        });
    }
}
